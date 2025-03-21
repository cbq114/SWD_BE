using Tutor.Domains.Entities;
using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Interfaces
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<Bookings>> GetBookingsAsync();
        Task<Bookings> GetBookingByIdAsync(int bookingId);
        Task UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task DeleteBookingAsync(int bookingId);

        Task<IEnumerable<Payments>> GetPaymentsAsync();
        Task<IEnumerable<Refunds>> GetRefundsAsync();
        Task<Refunds> GetRefundByIdAsync(int refundId);
        Task<IEnumerable<Payouts>> GetPayoutsAsync();
        Task<IEnumerable<Feedbacks>> GetFeedbacksAsync();
        Task DeleteFeedbackAsync(int feedbackId);
        Task<IEnumerable<Payouts>> GetPayoutsByUserNameAsync(string username);
        Task<object> GetDashboardSummaryAsync();
        Task<IEnumerable<object>> GetMonthlyRevenueAsync(int year);

        Task<IEnumerable<object>> GetUserDistributionAsync();
        Task<IEnumerable<object>> GetUserGrowthAsync(int year);
    }
}