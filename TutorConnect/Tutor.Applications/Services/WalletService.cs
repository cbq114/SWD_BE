using Microsoft.Extensions.Configuration;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Shared.Helper;

namespace Tutor.Applications.Services
{
    public class WalletService : IWalletService
    {
        private readonly IConfiguration _configuration;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository, IUserRepository userRepository, IConfiguration configuration, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task autoPaidForTutor()
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var now = DateTimeHelper.GetVietnamNow();
                var aDayAgo = now.AddDays(-1); // Auto paid for Tutor after 1 day

                var succesPayments = await _paymentRepository.GetAllNotPaidSuccessPayment(aDayAgo);
                if (!succesPayments.Any())
                    return;

                var rate = Convert.ToDouble(_configuration["RatePrice:rate"]);

                foreach (var payment in succesPayments)
                {
                    var admin = await _userRepository.GetAdmin();
                    if (admin == null)
                        return;
                    // update wallet and create save transaction
                    var wallet = await _walletRepository.GetWalletByUsername(payment.Booking.TutorAvailability.Instructor);
                    var amount = payment.Amount * (1 - rate);   //rate
                    var description = $"Payout for tutor {payment.Booking.TutorAvailability.Instructor} from customer {payment.Booking.customer} for booking session {payment.BookingId}.";
                    if (wallet == null)
                        throw new Exception("Error: Cannot found wallet while excute payment");

                    // Add money into wallet and create transaction
                    await _walletRepository.AddMoney(wallet.UserName, (double)amount);
                    await _transactionRepository.AddTransaction(wallet.WalletId, (double)amount, description, payment.PaymentCode);

                    // Add to admin wallet
                    var adminWallet = await _walletRepository.GetWalletByUsername(admin.UserName);
                    if (adminWallet == null)
                        Console.WriteLine("Error: Cannot found wallet of admin while excute payment");
                    var adminAmount = payment.Amount * rate;
                    var admindescription = $"Commission fee received from tutor {payment.Booking.TutorAvailability.Instructor} for booking session {payment.BookingId}.";

                    if (adminWallet == null)
                        throw new Exception("Dont have any admin in this dtb!");

                    // Add money into admin wallet and create transaction
                    await _walletRepository.AddMoney(admin.UserName, (double)adminAmount);
                    await _transactionRepository.AddTransaction(adminWallet.WalletId, (double)adminAmount, admindescription, payment.PaymentCode);

                    // Mark payment as paided
                    payment.IsPaid = true;
                    await _paymentRepository.UpdatePayment(payment);

                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackAsync();
                Console.WriteLine($"Error while auto paid for tutor: {e}");
            }
        }
    }
}
