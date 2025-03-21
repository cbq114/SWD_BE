using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.HomeInstructorModel;

namespace Tutor.Applications.Services
{
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;

        public HomeService(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        public Task<InstructorDetailsModelHome> GetInstructorDetailsByIdAsync(string instructorId)
        {
            return _homeRepository.GetInstructorDetailsByIdAsync(instructorId);
        }

        public Task<List<InstructorDetailsModelHome>> GetInstructorsForHomepageAsync(InstructorSearchOptions searchOptions)
        {
            return _homeRepository.GetInstructorsForHomepageAsync(searchOptions);
        }
    }
}
