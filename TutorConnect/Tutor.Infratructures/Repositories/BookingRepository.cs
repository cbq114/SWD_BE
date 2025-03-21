using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class BookingRepository : Repository<Bookings>, IBookingRepository
    {
        private readonly TutorDBContext _dbContext;
        public BookingRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Bookings> GetBookingById(int id)
        {
            var booking = await Entities
                .Include(b => b.Lesson)
                .Include(b => b.TutorAvailability)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null)
                Console.WriteLine(new ArgumentNullException(nameof(booking), $"Can not find booking with id = {id}"));

            return booking;
        }
        public async Task<List<Bookings>> GetActiveBookingsForTutorAsync(string instructorUsername, DateTime currentTime)
        {
            try
            {
                return await _dbContext.Bookings
                    .Include(b => b.TutorAvailability)
                    .Where(b => b.Lesson.Instructor == instructorUsername &&
                           (b.Status == BookingStatus.Accepted || b.Status == BookingStatus.InProgress) &&
                           b.TutorAvailability.StartTime <= currentTime.AddMinutes(15) &&
                           b.TutorAvailability.EndTime >= currentTime)
                      .OrderByDescending(b => b.Created)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Bookings>();
            }
        }

        public async Task<int> CreateBooking(Bookings booking)
        {
            Entities.Add(booking);
            await _dbContext.SaveChangesAsync();
            return booking.BookingId;
        }

        public async Task<bool> IsAvailabilityBooked(int availabilityId)
        {
            return await Entities.AnyAsync(b => b.AvailabilityId == availabilityId && b.Status == BookingStatus.Accepted);
        }

        public async Task ChangeBookingStatus(int bookingId, BookingStatus status)
        {
            var booking = await GetBookingById(bookingId);
            booking.Status = status;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Bookings>> GetAllPendingBooking()
        {
            return await Entities.Where(b => b.Status == BookingStatus.Pending).ToListAsync();
        }

        public async Task<List<Bookings>> GetAllBookingOfUser(string username)
        {
            return await Entities
                .Include(b => b.TutorAvailability)
                .Include(b => b.Lesson)
                .Where(b => b.customer == username)
                .ToListAsync();
        }

        public async Task<List<Bookings>> GetAllBookingTutor(string username)
        {
            return await Entities
                .Include(b => b.TutorAvailability)
                .Where(b => b.TutorAvailability.Instructor == username).OrderByDescending(b => b.Created)
                .ToListAsync();
        }

        public async Task<bool> CheckUserUsedTolearnedTutor(string username, string tutor)
        {
            var booking = await Entities
                .Include(b => b.TutorAvailability)
                .FirstOrDefaultAsync(b => b.customer == username && b.TutorAvailability.Instructor == tutor && b.Status == BookingStatus.Completed);
            if (booking == null)
                return false;
            return true;
        }

        public async Task<bool> CheckUserUsedTolearnedLesson(string username, int lessonId)
        {
            var booking = await Entities
    .FirstOrDefaultAsync(b => b.customer == username && b.LessonId == lessonId && b.Status == BookingStatus.Completed);
            return booking != null;
        }

        public async Task UpdateBooking(Bookings bookings)
        {
            Entities.Update(bookings);
            await _dbContext.SaveChangesAsync();
        }
    }
}
