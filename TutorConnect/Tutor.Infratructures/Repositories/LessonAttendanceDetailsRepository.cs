using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class LessonAttendanceDetailsRepository : Repository<LessonAttendanceDetails>, ILessonAttendanceDetailsRepository
    {
        private readonly TutorDBContext _dbContext;
        public LessonAttendanceDetailsRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<LessonAttendanceDetails>> GetByAttendanceIdAsync(int attendanceId)
        {
            return await Entities
                .Where(detail => detail.LessonAttendanceId == attendanceId)
                .ToListAsync();
        }
    }
}