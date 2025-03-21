using Microsoft.AspNetCore.Http;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Authen;

namespace Tutor.Applications.Interfaces
{
    public interface IAuthenService
    {
        Task<(string, string)> Login(LoginModel model);
        Task<(string, string)> LoginWithRefreshToken(string refreshToken);
        Task<string> RegisterStudent(RegisterBaseModel model, IFormFile avatarFile);
        Task<string> RegisterInstructor(RegisterInstructorModel model);
        string GenerateJwtToken(Users user);
        Task<bool> Logout(HttpContext httpContext);
        Task<Users> GetUserByEmail(string email);
        Task<string> ConfirmEmailAsync(string? username);
    }
}
