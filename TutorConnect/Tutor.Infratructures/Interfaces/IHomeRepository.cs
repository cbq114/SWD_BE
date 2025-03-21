using Tutor.Infratructures.Models.HomeInstructorModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface IHomeRepository
    {
        Task<List<InstructorDetailsModelHome>> GetInstructorsForHomepageAsync(InstructorSearchOptions searchOptions);
        Task<InstructorDetailsModelHome> GetInstructorDetailsByIdAsync(string instructorId);
    }
}
