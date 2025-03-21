using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;

namespace Tutor.Infratructures.Interfaces
{
    public interface ILessonAttendanceRepository : IRepository<LessonAttendances>
    {
        Task<LessonAttendances> GetByBookingIdAsync(int bookingId);
        Task<List<LessonAttendances>> GetAllByBookingIdsAsync(List<int> bookingIds);
        Task<List<int>> GetActiveBookingIdsAsync();
        Task<List<int>> GetBookingIdsStartingSoonAsync(DateTime threshold);
        Task<List<int>> GetBookingIdsWithoutStudentJoinAsync(DateTime startedBefore);
        Task<List<int>> GetBookingIdsPastEndTimeAsync(DateTime now);
    }
}
