using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.AvailiabilityModel;

namespace Tutor.Applications.Services
{
    public class TutorScheduleService : ITutorScheduleService
    {
        private readonly ITutorAvailabilitityRepository _repository;

        public TutorScheduleService(ITutorAvailabilitityRepository repository)
        {
            _repository = repository;
        }

        public Task<string> AddTutorAvailability(CreateTutorAvailabilityModel model, string instructor)
        {
            return _repository.AddTutorAvailability(model, instructor);
        }

        public Task<bool> DeleteTutorAvailability(string tutor, int id)
        {
            return _repository.DeleteTutorAvailability(tutor, id);
        }

        public Task<List<ScheduleModel>> GetAllScheduleforStudent(string instructor)
        {
            return _repository.GetAllScheduleforStudentView(instructor);
        }

        public Task<List<ScheduleModel>> GetAllScheduleforTutor(string instructor)
        {
            return _repository.GetAllScheduleforTutorView(instructor);
        }

        public Task<TutorAvailabilities> getScheduleById(int id)
        {
            return _repository.GetScheduleById(id);
        }

        public Task<TutorAvailabilities> UpdateTutorAvailability(CreateTutorAvailabilityModel model, int id)
        {
            return _repository.UpdateTutorAvailability(model, id);
        }
        public Task ChangeTutorAvailStatus(int id, TutorAvailabilitityStatus status)
        {
            return _repository.ChangeTutorAvailStatus(id, status);
        }
    }
}
