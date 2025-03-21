using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Authen;

namespace Tutor.Infratructures.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> EmailSendAsync(string email, string subject, string message);
        //string GetMailBodyUpdateProfile(string userName, UserProfileUpdateModel updateModel);

        string GetMailBody(RegisterBaseModel model);
        Task<Users> GetUserByUsernameAsync(string username);
        Task UpdateUserAsync(Users user);
        Task<string> ConfirmEmailAsync(string username);
        //string GetMailBodyUpdatePassword(UpdatePasswordModel updatePassword);

    }
}
