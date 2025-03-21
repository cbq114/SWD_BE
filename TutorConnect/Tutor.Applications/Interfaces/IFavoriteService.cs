using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Interfaces
{
    public interface IFavoriteService
    {
        Task<List<UserViewInstructorModel>> GetFavoriteInstructors(string userName);
        Task<bool> AddFavoriteInstructor(string userName, string instructorId);
        Task<bool> RemoveFavoriteInstructor(string userName, string instructorId);
    }
}
