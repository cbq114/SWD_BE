using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IPaypalService _paypalService;
        private readonly IConfiguration _configuration;
        public PaymentController(IPaymentService paymentService, IUserService userService, IPaypalService paypalService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _userService = userService;
            _paypalService = paypalService;
            _configuration = configuration;
        }

        [HttpPost("pay")]
        [Authorize]
        public async Task<IActionResult> MakePayment([FromQuery] int bookingId, [FromQuery] int promotionId)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized(ApiResponse<string>.ErrorResult("User is not authenticated"));

                var user = await _userService.UserViewInstructorModel(username);
                if (user == null)
                    return NotFound(ApiResponse<string>.ErrorResult("User not found"));

                var result = await _paypalService.CreatePaymentUrl(bookingId, promotionId);

                if (result.Contains("Insufficient balance"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            } catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while generate payment: {ex.Message}"));
            }
        }

        [HttpGet("invoice")]
        [Authorize]
        public async Task<IActionResult> GetInvoice([FromQuery] int bookingId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return NotFound(ApiResponse<string>.ErrorResult("User not found"));

            try
            {
                var invoice = await _paymentService.GetInvoice(bookingId, username);
                return Ok(ApiResponse<InvoiceDTO>.SuccessResult(invoice));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Error fetching invoice: " + ex.Message));
            }
        }

        [HttpPost("create-refund-request")]
        [Authorize]
        public async Task<IActionResult> RefundPayment([FromBody] RefundRequestDTO request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return NotFound(ApiResponse<string>.ErrorResult("User not found"));

            try
            {
                var result = await _paypalService.CreateRefundRequest(request.BookingId, request.Reason, username);
                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Create refund request failed: " + ex.Message));
            }
        }

        [HttpGet("get-all-refund")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllPendingRefund()
        {
            try
            {
                var result = await _paymentService.GetRefundRequest();
                if (!result.Any())
                    return BadRequest(ApiResponse<string>.ErrorResult("There is no pending refund"));

                return Ok(ApiResponse<List<Refunds>>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Get pending refund failed: " + ex.Message));
            }
        }

        [HttpGet("get-refund/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllPendingRefund([FromRoute] int id )
        {
            try
            {
                var result = await _paymentService.GetRefundById(id);
                if (result == null)
                    return BadRequest(ApiResponse<string>.ErrorResult($"There is no refund with id: {id}"));

                return Ok(ApiResponse<Refunds>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Get refund failed: " + ex.Message));
            }
        }

        [HttpPut("accept-refund")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RefundPayment([FromQuery] int refundId)
        {
            try
            {
                var result = await _paypalService.RefundPayment(refundId);
                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Refund failed: " + ex.Message));
            }
        }
        [HttpPut("deny-refund")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DenyRefundPayment([FromQuery] int refundId)
        {
            try
            {
                var result = await _paypalService.denyRefundRequest(refundId);
                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Error while deny refund: " + ex.Message));
            }
        }

        [HttpGet("paypal_return")]
        public async Task<IActionResult> PayPalReturn([FromQuery] string order_id, [FromQuery] string token)
        {
            try
            {
                // ** Gọi PaymentExecute để xử lý thanh toán **
                var bookingId = await _paypalService.PaymentExecute(order_id, token);
                var frontUrl = _configuration["Paypal:FrontUrl"];

                if (bookingId > 0)
                {
                    // Redirect đến trang thông báo thanh toán thành công
                    return Redirect($"{frontUrl}/payment-success/{bookingId}");
                    //return Ok(ApiResponse<string>.SuccessResult("Success"));
                }
                else
                {
                    // Redirect đến trang thông báo thanh toán thất bại
                    return BadRequest(ApiResponse<string>.ErrorResult("failed"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Payment execution failed: {ex.Message}"));
            }
        }

        [HttpGet("get_all_of_user")]
        public async Task<IActionResult> GetAllPaymentOfUser()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (username == null)
                    return NotFound(ApiResponse<string>.ErrorResult("User not found"));

                var payments = await _paymentService.GetAllPaymentOfUser(username);
                if (payments.Count == 0)
                    return NotFound(ApiResponse<string>.ErrorResult("Payment not found"));

                return Ok(ApiResponse<List<PaymentDTO>>.SuccessResult(payments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Payment get failed: {ex.Message}"));
            }
        }

    }
}
