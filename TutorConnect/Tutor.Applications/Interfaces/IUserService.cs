using Microsoft.AspNetCore.Http;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Interfaces
{
    public interface IUserService
    {
        Task<(bool success, string message, Users? user)> InitiateForgotPasswordAsync(string email);
        Task<(bool success, string message)> VerifyCodeAsync(string email, string code);
        Task<(bool success, string message)> ResetPasswordAsync(string email, string newPassword);
        Task<UserViewModel> GetByEmailAsync(string email);
        Task<UserViewModel?> GetByUsernameAsync(string username);

        Task<UserViewInstructorModel> UserViewInstructorModel(string username);
        Task<UserViewInstructorModel> GetUserViewInstructorModelByEmail(string email);

        Task<List<UserViewInstructorModel>> GetAllInstructors();
        Task<List<UserViewModel>> GetAllStudents();

        Task<List<UserViewInstructorModel>> GetFilteredInstructors(SearchOptionUser searchOption);
        Task<string> UpdateTutorProfile(string username, UpdateTutor updateTutor);
        Task<string> UpdateUserProfile(string username, UpdateUser updateUser);
        Task<string> UpdateAvatar(string userName, IFormFile avatarFile);
        Task<Users> GetCurrentUser(string username);
        Task<Users> GetUserByEmail(string email);
        Task<Users> CreateUser(Users user);
        Task<bool> CheckFavoriteInstructor(string userName);
        Task<bool> CheckWalletExists(string userName);
        Task<string> GenerateJWTTOKEN(Users user);
        Task<Users?> GetTargetUserInRoom(int roomId, string currentUsername);
        Task<bool> DeleteUserAsync(string username);
        Task<List<UserModelDTO>> GetAllAccount(UserFilter userFilter);
        Task<string> BanAccount(string username);
        Task<string> UpdateRole(string username, int rolesId);
        Task<bool> BlockAccount(string username);
        Task<bool> UnBlockAccount(string username);
    }
}
