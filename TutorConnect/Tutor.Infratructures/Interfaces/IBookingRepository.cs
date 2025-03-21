using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;

namespace Tutor.Infratructures.Interfaces
{
    public interface IBookingRepository : IRepository<Bookings>
    {
        Task<Bookings> GetBookingById(int id);
        Task<int> CreateBooking(Bookings booking);
        Task<bool> IsAvailabilityBooked(int availabilityId);
        Task ChangeBookingStatus(int bookingId, BookingStatus status);
        Task<List<Bookings>> GetAllPendingBooking();
        Task<List<Bookings>> GetAllBookingOfUser(string username);
        Task<List<Bookings>> GetAllBookingTutor(string username);
        Task<bool> CheckUserUsedTolearnedTutor(string username, string tutor);
        Task<List<Bookings>> GetActiveBookingsForTutorAsync(string instructorUsername, DateTime currentTime);
        Task<bool> CheckUserUsedTolearnedLesson(string username, int lessonId);
        Task UpdateBooking(Bookings bookings);
    }
}
