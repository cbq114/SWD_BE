using Tutor.Infratructures.Models.HomeInstructorModel;

namespace Tutor.Applications.Interfaces
{
    public interface IHomeService
    {
        Task<List<InstructorDetailsModelHome>> GetInstructorsForHomepageAsync(InstructorSearchOptions searchOptions);
        Task<InstructorDetailsModelHome> GetInstructorDetailsByIdAsync(string instructorId);
    }
}
