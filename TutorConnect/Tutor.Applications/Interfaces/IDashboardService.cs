using Tutor.Infratructures.Models.DashboardModel;

namespace Tutor.Applications.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<BookingViewModel>> GetBookingsAsync();
        Task<BookingViewModel> GetBookingByIdAsync(int bookingId);
        Task UpdateBookingStatusAsync(int bookingId, string status);
        Task DeleteBookingAsync(int bookingId);
        Task<IEnumerable<PaymentViewModel>> GetPaymentsAsync();
        Task<IEnumerable<RefundViewModel>> GetRefundsAsync();
        Task<RefundViewModel> GetRefundByIdAsync(int refundId);
        Task<IEnumerable<PayoutViewModel>> GetPayoutsAsync();
        Task<IEnumerable<FeedbackViewModel>> GetFeedbacksAsync();
        Task DeleteFeedbackAsync(int feedbackId);
        Task<IEnumerable<PayoutViewModel>> GetPayoutsByUserNameAsync(string username);

        Task<object> GetDashboardSummaryAsync();
        Task<IEnumerable<object>> GetMonthlyRevenueAsync(int year);

        Task<IEnumerable<object>> GetUserDistributionAsync();
        Task<IEnumerable<object>> GetUserGrowthAsync(int year);
    }
}
