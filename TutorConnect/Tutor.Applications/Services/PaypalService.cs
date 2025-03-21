using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.PaypalModel;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;
using Money = PayPalCheckoutSdk.Payments.Money;
using Refund = PayPalCheckoutSdk.Payments.Refund;
namespace Tutor.Applications.Services
{
    public class PaypalService : IPaypalService
    {
        private readonly PayPalHttpClient _payPalClient;
        private readonly IConfiguration _configuration;
        private readonly GetBalancePayPal _options;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITutorAvailabilitityRepository _tavailabilitityRepository;
        private readonly IRefundRepository _refundRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionUsageRepository _promotionUsageRepository;
        private readonly HttpClient _httpClient;
        private readonly TutorDBContext _dbContext;
        public PaypalService(IConfiguration configuration, IPaymentRepository paymentRepository, IBookingRepository bookingRepository, ITutorAvailabilitityRepository tutorAvailabilitityRepository, IRefundRepository refundRepository, IWalletRepository walletRepository, ITransactionRepository transactionRepository, IPromotionRepository promotionRepository, IPromotionUsageRepository promotionUsageRepository, IHttpClientFactory httpClientFactory, TutorDBContext tutorDBContext)
        {
            _configuration = configuration;
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _promotionRepository = promotionRepository;
            _promotionUsageRepository = promotionUsageRepository;
            _tavailabilitityRepository = tutorAvailabilitityRepository;

            var environment = new SandboxEnvironment(
                _configuration["Paypal:ClientId"],
                _configuration["Paypal:SecretKey"]
            );
            _httpClient = httpClientFactory.CreateClient();
            _payPalClient = new PayPalHttpClient(environment);
            _refundRepository = refundRepository;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _dbContext = tutorDBContext;
        }
        //
        public async Task<decimal?> GetBalanceAsync()
        {
            var requestParams = new Dictionary<string, string>
    {
        { "METHOD", "GetBalance" },
        { "USER", _configuration["PaypalNVP:UserName"] },
        { "PWD", _configuration["PaypalNVP:Password"] },
        { "SIGNATURE", _configuration["PaypalNVP:Signature"] },
        { "VERSION", "204.0" },
        { "RETURNALLCURRENCIES", "1" }
    };

            var requestBody = new StringBuilder();
            foreach (var param in requestParams)
            {
                requestBody.Append($"{param.Key}={HttpUtility.UrlEncode(param.Value)}&");
            }

            var content = new StringContent(requestBody.ToString().TrimEnd('&'), Encoding.UTF8, "application/x-www-form-urlencoded");
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync("https://api-3t.sandbox.paypal.com/nvp", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var responseValues = ParseNvpResponse(responseBody);
                if (responseValues.TryGetValue("L_AMT0", out var balance) && decimal.TryParse(balance, out var balanceValue))
                {
                    return balanceValue;
                }
            }
            return null;
        }

        private Dictionary<string, string> ParseNvpResponse(string response)
        {
            return response.Split('&')
                .Select(x => x.Split('='))
                .ToDictionary(
                    x => HttpUtility.UrlDecode(x[0]),
                    x => HttpUtility.UrlDecode(x[1])
                );
        }

        //yêu cầu payout
        public async Task<PaymentResult> RequestPayoutAsync(string tutorEmail, double amount, string reason)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == tutorEmail);
            if (user == null)
            {
                return new PaymentResult { Success = false, Message = "User not found" };
            }

            var wallet = await _dbContext.Wallet.SingleOrDefaultAsync(w => w.UserName == user.UserName);
            if (wallet == null)
            {
                return new PaymentResult { Success = false, Message = "Wallet not found" };
            }

            if (wallet.Balance < amount)
            {
                return new PaymentResult { Success = false, Message = $"Not enough balance in your wallet. Your wallet currently has {wallet.Balance}$" };
            }

            var payout = new Domains.Entities.Payouts
            {
                UserName = user.UserName,
                Amount = amount,
                PayoutDate = DateTimeHelper.GetVietnamNow(),
                Status = PayoutStatus.Pending,
                Reason = reason,
            };

            await _dbContext.Payouts.AddAsync(payout);
            await _dbContext.SaveChangesAsync();
            return new PaymentResult { Success = true, Message = $"Payout request for {amount:F2} has been submitted and is pending approval." };
        }

        // Approve
        public async Task<PaymentResult> ProcessApprovedPayoutAsync(int payoutId, bool accept, string comment)
        {
            var payout = await _dbContext.Payouts.SingleOrDefaultAsync(p => p.PayoutId == payoutId && p.Status == PayoutStatus.Pending);
            if (payout == null)
                return new PaymentResult { Success = false, Message = "Pending payout not found." };

            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == payout.UserName);
            var email = "";

            if (user != null)
            {
                email = user.Email;
            }


            if (!accept)
            {
                payout.Status = PayoutStatus.Rejected;
                payout.PayoutDate = DateTimeHelper.GetVietnamNow();
                await _dbContext.SaveChangesAsync();

                var response = new PayoutResponses
                {
                    PayoutId = payout.PayoutId,
                    IsApproved = false,
                    ResponseDate = DateTimeHelper.GetVietnamNow(),
                    Comment = comment
                };

                await _dbContext.PayoutResponses.AddAsync(response);
                await _dbContext.SaveChangesAsync();

                return new PaymentResult { Success = false, Message = "Payout request rejected by admin." };
            }
            var clientId = _configuration["Paypal:ClientId"];
            var secretKey = _configuration["Paypal:SecretKey"];
            var baseUrl = _configuration["Paypal:BaseUrl"];

            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{secretKey}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

            var tokenResponse = await _httpClient.PostAsync($"{baseUrl}/v1/oauth2/token", new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
            }));

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return new PaymentResult { Success = false, Message = "Failed to obtain access token." };
            }

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var accessToken = JObject.Parse(tokenContent)["access_token"]?.ToString();

            if (string.IsNullOrEmpty(accessToken))
            {
                return new PaymentResult { Success = false, Message = "Failed to retrieve access token." };
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var transferRequest = new
            {
                sender_batch_header = new
                {
                    sender_batch_id = Guid.NewGuid().ToString(),
                    email_subject = "You have a payout",
                    email_message = "You have received a payout! Thanks for using our service!"

                },
                items = new[]
                              {
              new
                {
                recipient_type = "EMAIL",
                amount = new { value = payout.Amount.ToString("F2"), currency = "USD" },
                receiver = email,
                note = "Thank you for your service!",
                sender_item_id = "item_1"
                 }
                               }
            };

            var transferResponse = await _httpClient.PostAsJsonAsync($"{baseUrl}/v1/payments/payouts", transferRequest);


            if (transferResponse.IsSuccessStatusCode)
            {
                var wallet = await _dbContext.Wallet.SingleOrDefaultAsync(w => w.UserName == payout.UserName);
                if (wallet != null)
                {
                    wallet.Balance -= payout.Amount;
                    payout.Status = PayoutStatus.Completed;
                }
                var response = new PayoutResponses
                {
                    PayoutId = payout.PayoutId,
                    IsApproved = true,
                    ResponseDate = DateTimeHelper.GetVietnamNow(),
                    Comment = comment,
                };
                var transaction = new Transactions
                {
                    walletId = wallet.WalletId,
                    OrderCode = GeneratePayoutOrderCode(),
                    CreatedDate = DateTimeHelper.GetVietnamNow(),
                    Amount = payout.Amount,
                    Description = $"Customer {payout.UserName} Have payout with amount {payout.Amount}$",
                    Status = "Success"
                };
                await _dbContext.Transactions.AddAsync(transaction);
                await _dbContext.PayoutResponses.AddAsync(response);
                await _dbContext.SaveChangesAsync();

                return new PaymentResult { Success = true, Message = "Payout approved and processed successfully." };
            }
            else
            {
                payout.Status = PayoutStatus.Failed;
                await _dbContext.SaveChangesAsync();

                return new PaymentResult { Success = false, Message = $"Payout processing failed: {await transferResponse.Content.ReadAsStringAsync()}" };
            }
        }
        private string GeneratePayoutOrderCode()
        {
            return $"Payout-{DateTimeHelper.GetVietnamNow():yyyyMMddHHmmss}";
        }
        //
        public async Task<string> CreatePaymentUrl(int bookingId, int promotionId)
        {
            var booking = await _bookingRepository.GetBookingById(bookingId);
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString(); // Lấy timestamp
            var paypalOrderId = $"{bookingId}-{timestamp}";  // Kết hợp BookingId và timestamp
            var urlCallBack = _configuration["PaypalPaymentCallBack:ReturnUrl"];
            var urlCancel = _configuration["Paypal:FrontUrl"];
            var now = DateTimeOffset.Now;

            if (booking == null)
                return $"Cannot found booking with id: {bookingId}";
            if (booking.Status != BookingStatus.Pending)
                return $"Cannot process this booking with id: {bookingId}";

            double price = (double)booking.Price;

            if (promotionId != 0)
            {
                var promotion = await _promotionRepository.GetById(promotionId);
                if (promotion == null || promotion.Discount <= 0 || promotion.Discount > 100)
                    throw new Exception($"Invalid promotion with id: {promotionId}");

                // Check if promotion is ready to use
                if (promotion.StartDate > now || promotion.EndDate < now)
                    throw new Exception("This promotion is not in valid time to use!");

                if (promotion.limit <= 0 || promotion.Status == PromotionStatus.Inactive)
                    throw new Exception("This promotion is out and can not be used");

                var proUsage = new PromotionUsage()
                {
                    promotionId = promotionId,
                    bookingId = bookingId,
                    DiscountAmount = (decimal)(price * (double)promotion.Discount / 100)
                };
                await _promotionUsageRepository.Create(proUsage);
                await _promotionRepository.UsePromotion(promotionId);

                price -= price * (double)promotion.Discount / 100;
            }
            // Check xem đã tao payment trước đó chưa
            // Payment will auto failed after 10 minutes and GetPaymentByBookingId only get pending payment
            var payment = await _paymentRepository.GetPaymentByBookingId(bookingId);
            if (payment == null)
            {
                // ** Lưu Payment với trạng thái PENDING **
                var newPayment = new Payments
                {
                    BookingId = bookingId,
                    CreatedDate = DateTimeHelper.GetVietnamNow(),
                    Amount = price,
                    Description = booking.Note,
                    Status = PaymentStatus.Pending
                };
                await _paymentRepository.CreatePayment(newPayment);
            }

            // ** Tạo yêu cầu thanh toán trên PayPal **
            var order = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        ReferenceId = paypalOrderId,
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value = price.ToString()
                        },
                        Description = $"Invoice #{booking.Lesson.Title}"
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = $"{urlCallBack}payment/paypal_return?success=1&order_id={paypalOrderId}",
                    CancelUrl = $"{urlCancel}paymentCancel"
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);

            try
            {
                var response = await _payPalClient.Execute(request);

                if (response.StatusCode != HttpStatusCode.Created)
                    return "Error: No approval link found from PayPal.";

                var result = response.Result<Order>();

                foreach (var link in result.Links)
                {
                    if (link.Rel == "approve")
                    {
                        return link.Href;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error: An error occurred while creating PayPal order - {ex.Message}";
            }
        }

        public async Task<int> PaymentExecute(string order_id, string token)
        {
            if (string.IsNullOrEmpty(order_id) || string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Error: Missing token from PayPal.");
                return 0;
            }

            int bookingId;
            string[] parts = order_id.Split('-');
            if (parts.Length > 0 && int.TryParse(parts[0], out bookingId))
            {
                // bookingId đã được lấy thành công
            }
            else
            {
                Console.WriteLine($"Error: Invalid token format: {order_id}");
                return 0;
            }

            var booking = await _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                Console.WriteLine($"Error: Booking ID {bookingId} not found.");
                return 0;
            }

            var payment = await _paymentRepository.GetPaymentByBookingId(bookingId);
            if (payment == null)
            {
                Console.WriteLine($"Error: Payment for Booking ID {bookingId} not found.");
                return 0;
            }

            try
            {
                // Lấy thông tin Order từ PayPal
                var order = await GetOrderDetails(token);
                if (order == null)
                {
                    Console.WriteLine($"Error: Unable to retrieve order details for token: {token}");
                    return 0;
                }

                payment.PaymentCode = order.Id;
                payment.Status = PaymentStatus.Success;
                booking.Price = payment.Amount;
                await _bookingRepository.UpdateBooking(booking);
                await _paymentRepository.UpdatePayment(payment);
                //// update wallet and create save transaction
                //var wallet = await _walletRepository.GetWalletByUsername(booking.TutorAvailability.Instructor);
                //var amount = payment.Amount * 70 / 100;
                //var description = $"Payment of {booking.customer} for booking";
                //if (wallet == null)
                //    Console.WriteLine("Error: Cannot found wallet while excute payment");

                //await _walletRepository.AddMoney(wallet.UserName,(double) amount);
                //await _transactionRepository.AddTransaction(wallet.WalletId, (double)amount, description);

                await _bookingRepository.ChangeBookingStatus(bookingId, BookingStatus.Accepted);
                await _tavailabilitityRepository.ChangeTutorAvailStatus(booking.AvailabilityId, TutorAvailabilitityStatus.booked);

                return bookingId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing PayPal payment: {ex.Message}");
                return 0;
            }
        }

        private async Task<Order> GetOrderDetails(string token)
        {
            try
            {
                // Lấy orderId từ token
                var request = new OrdersGetRequest(token);
                var response = await _payPalClient.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Error getting order details from PayPal. Status code: {response.StatusCode}");
                    return null;
                }

                Order order = response.Result<Order>();
                string paypalOrderId = order.Id;

                // Lấy thông tin chi tiết với orderId chính xác
                var orderRequest = new OrdersGetRequest(paypalOrderId);
                var orderResponse = await _payPalClient.Execute(orderRequest);

                if (orderResponse.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Error getting order details from PayPal. Status code: {orderResponse.StatusCode}");
                    return null;
                }

                return orderResponse.Result<Order>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order details: {ex.Message}");
                return null;
            }
        }

        public async Task<string> CreateRefundRequest(int bookingId, string reason, string username)
        {
            var payments = await _paymentRepository.GetByBookingId(bookingId);
            var booking = await _bookingRepository.GetBookingById(bookingId);

            if (booking.customer != username)
                return "Error: You can not refund for others account";

            if (payments == null || booking == null)
                return "Error: This booking ID is not valid to refund!";

            if (booking.Status != BookingStatus.Accepted || payments.Status != PaymentStatus.Success)
                return "Error: This booking is not pay yet!";

            // Refund only can process before 1 hour after process payment
            var now = DateTimeHelper.GetVietnamNow();
            var anHoursAgo = now.AddHours(-1);
            if (payments.CreatedDate <= anHoursAgo)
                return "Error: Payment too late to refund (more than 1 hour)";

            if (!payments.Amount.HasValue)
                return "Error: Payment is not eligible for refund.";

            var refundPayment = new Refunds
            {
                BookingId = bookingId,
                RefundDate = DateTimeHelper.GetVietnamNow(),
                Amount = (decimal)(payments.Amount * 0.8),
                Reason = reason,
                TransactionId = "",
                Status = RefundStatus.Pending,
            };
            await _refundRepository.CreateRefund(refundPayment);
            return "Your refund request has been send, please waiting for our response.";
        }

        public async Task<string> denyRefundRequest(int refundId)
        {
            var refund = await _refundRepository.GetRefundsById(refundId);
            if (refund == null)
                return $"Error: Cannot find refund with id: {refundId}";

            refund.Status = RefundStatus.Failed;
            var result = await _refundRepository.UpdateRefund(refund);
            return "Deny success.";
        }

        public async Task<string> RefundPayment(int refundId)
        {
            var refund = await _refundRepository.GetRefundsById(refundId);
            var payment = await _paymentRepository.GetByBookingId(refund.BookingId);

            // Capture Order
            var captureId = await CaptureOrder(payment.PaymentCode);

            if (string.IsNullOrEmpty(captureId))
            {
                Console.WriteLine($"Error: Capture ID not found for order: {payment.PaymentCode}");
                return "Refund error: Cannot find capture id.";
            }

            var request = new CapturesRefundRequest(captureId);
            request.Prefer("return=representation");

            request.RequestBody(new RefundRequest()
            {
                Amount = new Money()
                {
                    CurrencyCode = "USD",
                    Value = $"{(double)refund.Amount:F2}" ?? "0.00"
                }
            });

            try
            {
                var response = await _payPalClient.Execute(request);
                var result = response.Result<Refund>();
                var tutorName = payment.Booking.TutorAvailability.Instructor;

                if (result != null)
                {
                    // ** Tạo Payment mới để ghi nhận giao dịch refund **
                    refund.Status = RefundStatus.Success;
                    refund.TransactionId = result.Id;


                    await _refundRepository.UpdateRefund(refund);
                    //await _walletRepository.DeductBalance(tutorName, (double)payment.Amount);
                    await _bookingRepository.ChangeBookingStatus(payment.BookingId, BookingStatus.Cancelled);
                    await _tavailabilitityRepository.ChangeTutorAvailStatus(payment.Booking.AvailabilityId, TutorAvailabilitityStatus.Available);
                }

                return "Refund seuccessfully";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refund error: {ex.Message}");
                return $"Refund failed: {ex.Message}";
            }
        }

        private async Task<string> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                var response = await _payPalClient.Execute(request);

                if (response.StatusCode != HttpStatusCode.Created) // PayPal trả về 201 nếu Capture thành công
                {
                    Console.WriteLine($"Error capturing order from PayPal. Status code: {response.StatusCode}");
                    return null;
                }

                var result = response.Result<Order>();

                // Lấy Capture ID từ phần purchase_units[0].payments.captures[0]
                var captureId = result.PurchaseUnits[0].Payments.Captures[0].Id;

                Console.WriteLine($"Capture ID: {captureId}");

                return captureId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing order: {ex.Message}");
                return null;
            }
        }

        //private static double ConvertVndToDollar(double vnd)
        //{
        //    const double ExchangeRate = 25.52499;
        //    return Math.Round(vnd / ExchangeRate, 2);
        //}
    }
}
