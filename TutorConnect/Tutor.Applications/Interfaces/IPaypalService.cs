using Tutor.Infratructures.Models.PaypalModel;

namespace Tutor.Applications.Interfaces
{
    public interface IPaypalService
    {
        Task<PaymentResult> RequestPayoutAsync(string instructorEmail, double amount, string reason);
        Task<PaymentResult> ProcessApprovedPayoutAsync(int payoutId, bool accept, string comment);
        Task<decimal?> GetBalanceAsync();
        Task<string> CreatePaymentUrl(int bookingId, int promotionId);
        Task<int> PaymentExecute(string order_id, string token);
        Task<string> CreateRefundRequest(int bookingId, string reason, string username);
        Task<string> denyRefundRequest(int refundId);
        Task<string> RefundPayment(int refundId);
    }
}
