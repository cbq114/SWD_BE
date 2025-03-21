using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.AvailiabilityModel;

namespace Tutor.Applications.Interfaces
{
    public interface ITutorScheduleService
    {
        Task<string> AddTutorAvailability(CreateTutorAvailabilityModel model, string instructor);

        Task<TutorAvailabilities> UpdateTutorAvailability(CreateTutorAvailabilityModel model, int id);
        Task<bool> DeleteTutorAvailability(string tutor, int id);
        Task<TutorAvailabilities> getScheduleById(int id);
        Task<List<ScheduleModel>> GetAllScheduleforTutor(string instructor);
        Task<List<ScheduleModel>> GetAllScheduleforStudent(string instructor);
        Task ChangeTutorAvailStatus(int id, TutorAvailabilitityStatus status);
    }
}
