using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;

namespace Tutor.Infratructures.Interfaces
{
    public interface ILessonAttendanceDetailsRepository : IRepository<LessonAttendanceDetails>
    {
        Task<List<LessonAttendanceDetails>> GetByAttendanceIdAsync(int attendanceId);
    }
}
