using Microsoft.AspNetCore.Http;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Authen;

namespace Tutor.Applications.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IAuthenRepository _repository;
        private readonly IEmailSender _emailSender;
        public AuthenService(IAuthenRepository repository, IEmailSender emailSender)
        {
            _repository = repository;
            _emailSender = emailSender;
        }

        public async Task<string> ConfirmEmailAsync(string? username)
        {
            return await _emailSender.ConfirmEmailAsync(username);
        }

        public string GenerateJwtToken(Users user)
        {
            return _repository.GenerateJwtToken(user);
        }

        public Task<Users> GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<(string, string)> Login(LoginModel model)
        {
            return _repository.Login(model);
        }

        public Task<(string, string)> LoginWithRefreshToken(string refreshToken)
        {
            return _repository.LoginWithRefreshToken(refreshToken);
        }

        public Task<bool> Logout(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<string> RegisterInstructor(RegisterInstructorModel model)
        {
            return _repository.RegisterInstructor(model);
        }

        public Task<string> RegisterStudent(RegisterBaseModel model, IFormFile avatarFile)
        {
            return _repository.RegisterStudent(model, avatarFile);
        }
    }
}
