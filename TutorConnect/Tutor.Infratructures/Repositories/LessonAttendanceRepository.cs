using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class LessonAttendanceRepository : Repository<LessonAttendances>, ILessonAttendanceRepository
    {
        private readonly TutorDBContext _dbContext;
        public LessonAttendanceRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LessonAttendances> GetByBookingIdAsync(int bookingId)
        {
            return await Entities
                .FirstOrDefaultAsync(la => la.BookingId == bookingId);
        }

        public async Task<List<LessonAttendances>> GetAllByBookingIdsAsync(List<int> bookingIds)
        {
            return await Entities
                .Where(la => bookingIds.Contains(la.BookingId))
                .ToListAsync();
        }

        public async Task<List<int>> GetActiveBookingIdsAsync()
        {
            var now = DateTimeHelper.GetVietnamNow();

            return await _dbContext.TutorAvailabilities
                .Join(_dbContext.Bookings,
                    avail => avail.TutorAvailabilityId,
                    booking => booking.AvailabilityId,
                    (avail, booking) => new { avail, booking })
                .Where(x => x.booking.Status == BookingStatus.Accepted &&
                           x.avail.StartTime <= now &&
                           x.avail.EndTime >= now)
                .Select(x => x.booking.BookingId)
            .ToListAsync();
        }

        public async Task<List<int>> GetBookingIdsStartingSoonAsync(DateTime threshold)
        {
            return await _dbContext.TutorAvailabilities
                .Join(_dbContext.Bookings,
                    avail => avail.TutorAvailabilityId,
                    booking => booking.AvailabilityId,
                    (avail, booking) => new { avail, booking })
                .Where(x => x.booking.Status == BookingStatus.Accepted &&
                           x.avail.StartTime > DateTimeHelper.GetVietnamNow() &&
                           x.avail.StartTime <= threshold)
                .Select(x => x.booking.BookingId)
                .ToListAsync();
        }

        public async Task<List<int>> GetBookingIdsWithoutStudentJoinAsync(DateTime startedBefore)
        {
            // Get booking IDs that have started but student hasn't joined
            var startedLessons = await _dbContext.LessonAttendances
                .Where(la => la.CreateAt <= startedBefore &&
                            (la.IsAttended == null || la.IsAttended == false))
                .Select(la => la.BookingId)
                .ToListAsync();
            return startedLessons;
        }

        public async Task<List<int>> GetBookingIdsPastEndTimeAsync(DateTime now)
        {
            return await _dbContext.TutorAvailabilities
                .Join(_dbContext.Bookings,
                    avail => avail.TutorAvailabilityId,
                    booking => booking.AvailabilityId,
                    (avail, booking) => new { avail, booking })
                .Join(_dbContext.LessonAttendances,
                    x => x.booking.BookingId,
                    la => la.BookingId,
                    (x, la) => new { x.avail, x.booking, la })
                .Where(x => x.la.IsAttended == true &&
                           x.avail.EndTime < now &&
                           x.la.note == null) // Not completed yet
                .Select(x => x.booking.BookingId)
                .ToListAsync();
        }
    }
}
