using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface IFavoriteRepository
    {
        Task InitializeFavoriteInstructors(string userName);
        Task<List<UserViewInstructorModel>> GetFavoriteInstructors(string userName);
        Task<bool> AddFavoriteInstructor(string userName, string instructorId);
        Task<bool> RemoveFavoriteInstructor(string userName, string instructorId);
    }
}
