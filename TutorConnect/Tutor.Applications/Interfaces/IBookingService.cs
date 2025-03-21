using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Applications.Interfaces
{
    public interface IBookingService
    {
        Task<int?> CreateBooking(CreateBookingDTO bookingDTO);
        Task AutoRejectPendingBookings();
        Task<List<BookingDTO>> GetAllBookingOfUser(string username);
        Task<List<BookingDTO>> GetAllBookingTutor(string username);
        Task<BookingDTO> GetBookingById(int bookingId);
        Task<bool> CancelBooking(int bookingId);
        Task<bool> CheckUserUsedTolearnedTutor(string username, string tutor);
        Task<bool> CheckUserUsedTolearnedLesson(string username, int lessonId);
    }
}
