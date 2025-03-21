using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.AvailiabilityModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface ITutorAvailabilitityRepository
    {
        Task<string> AddTutorAvailability(CreateTutorAvailabilityModel model, string instructor);

        Task<TutorAvailabilities> UpdateTutorAvailability(CreateTutorAvailabilityModel model, int id);

        Task<bool> DeleteTutorAvailability(string tutor, int id);

        Task<TutorAvailabilities> GetScheduleById(int id);

        Task<List<ScheduleModel>> GetAllScheduleforStudentView(string instructor);
        Task<List<ScheduleModel>> GetAllScheduleforTutorView(string instructor);
        Task ChangeTutorAvailStatus(int id, TutorAvailabilitityStatus status);
    }
}
