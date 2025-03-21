using Microsoft.AspNetCore.Http;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface IUserRepository
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

        Task<Users> GetUserByUserName(string email);
        Task<string> UpdateUserProfile(Users user);
        Task<string> UpdateTutorProfile(Users user);
        Task<string> UpdateAvatar(string usnerName, IFormFile avatarFile);
        Task<Users> GetCurrentUser(string username);
        Task<Users> GetUserByEmail(string email);
        Task<Users> CreateUser(Users user);
        Task<bool> CheckFavoriteInstructor(string userName);
        Task<bool> CheckWalletExists(string userName);
        Task<string> GenerateJWTTOKEN(Users user);
        Task<List<Users>> GetUsersInRoom(int roomId);
        Task<Users?> GetTargetUserInRoom(int roomId, string currentUsername);
        Task<bool> DeleteUserAsync(string username);
        Task<Users> GetAdmin();
        Task<Users> GetTutor(string username);

        Task<List<UserModelDTO>> GetAllAccount(UserFilter userFilter);
        Task<bool> UpdateAccountStatus(string username, UserStatus status);
        Task<bool> UpdateAccountRole(string username, int roleId);
        Task<bool> BlockAccount(string username);
        Task<bool> UnBlockAccount(string username);
    }
}