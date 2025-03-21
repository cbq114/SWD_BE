using AutoMapper;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.DashboardModel;

namespace Tutor.Applications.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;
        private readonly IMapper _mapper;

        public DashboardService(IDashboardRepository dashboardRepository, IMapper mapper)
        {
            _repository = dashboardRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<BookingViewModel>> GetBookingsAsync()
        {
            var bookings = await _repository.GetBookingsAsync();
            return _mapper.Map<IEnumerable<BookingViewModel>>(bookings);
        }

        public async Task<BookingViewModel> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _repository.GetBookingByIdAsync(bookingId);
            return _mapper.Map<BookingViewModel>(booking);
        }
        public async Task UpdateBookingStatusAsync(int bookingId, string status)
        {
            if (Enum.TryParse(status, true, out BookingStatus bookingStatus))
            {
                await _repository.UpdateBookingStatusAsync(bookingId, bookingStatus);
            }
        }

        public async Task DeleteBookingAsync(int bookingId)
            => await _repository.DeleteBookingAsync(bookingId);

        public async Task<IEnumerable<PaymentViewModel>> GetPaymentsAsync()
            => _mapper.Map<IEnumerable<PaymentViewModel>>(await _repository.GetPaymentsAsync());

        public async Task<IEnumerable<RefundViewModel>> GetRefundsAsync()
            => _mapper.Map<IEnumerable<RefundViewModel>>(await _repository.GetRefundsAsync());

        public async Task<RefundViewModel> GetRefundByIdAsync(int refundId)
            => _mapper.Map<RefundViewModel>(await _repository.GetRefundByIdAsync(refundId));
        public async Task<IEnumerable<PayoutViewModel>> GetPayoutsAsync()
            => _mapper.Map<IEnumerable<PayoutViewModel>>(await _repository.GetPayoutsAsync());

        public async Task<IEnumerable<FeedbackViewModel>> GetFeedbacksAsync()
              => _mapper.Map<IEnumerable<FeedbackViewModel>>(await _repository.GetFeedbacksAsync());

        public async Task DeleteFeedbackAsync(int feedbackId)
            => await _repository.DeleteFeedbackAsync(feedbackId);

        public async Task<IEnumerable<PayoutViewModel>> GetPayoutsByUserNameAsync(string username)
                  => _mapper.Map<IEnumerable<PayoutViewModel>>(await _repository.GetPayoutsByUserNameAsync(username));

        public Task<object> GetDashboardSummaryAsync()
        {
            return _repository.GetDashboardSummaryAsync();
        }

        public Task<IEnumerable<object>> GetMonthlyRevenueAsync(int year)
        {
            return _repository.GetMonthlyRevenueAsync(year);
        }

        public Task<IEnumerable<object>> GetUserDistributionAsync()
        {
            return _repository.GetUserDistributionAsync();
        }

        public Task<IEnumerable<object>> GetUserGrowthAsync(int year)
        {
            return _repository.GetUserGrowthAsync(year);
        }
    }
}
