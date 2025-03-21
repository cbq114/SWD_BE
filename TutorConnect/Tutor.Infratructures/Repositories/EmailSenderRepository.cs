using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class EmailSenderRepository : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderRepository> _logger;
        private readonly TutorDBContext _dbcontext;

        public EmailSenderRepository(IConfiguration configuration, ILogger<EmailSenderRepository> logger, TutorDBContext dbcontext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbcontext = dbcontext;
        }

        public async Task<string> ConfirmEmailAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                return "Invalid confirmation request.";
            }

            if (user.Status == UserStatus.verified)
            {
                return "Your email has already been verified.";
            }

            user.Status = UserStatus.verified;
            await _dbcontext.SaveChangesAsync();

            return "Your account has been successfully confirmed.";
        }

        public async Task<bool> EmailSendAsync(string email, string subject, string message)
        {
            bool status = false;
            try
            {
                var secretKey = _configuration["AppSettings:SecretKey"];
                var from = _configuration["AppSettings:EmailSettings:From"];
                var smtpServer = _configuration["AppSettings:EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["AppSettings:EmailSettings:Port"]);
                var enableSSL = bool.Parse(_configuration["AppSettings:EmailSettings:EnablSSL"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = message,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    Credentials = new NetworkCredential(from, secretKey),
                    EnableSsl = enableSSL
                };

                // Gửi email
                await smtpClient.SendMailAsync(mailMessage);
                status = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the email.");
                status = false;
            }
            return status;
        }

        public Task UpdateUserAsync(Users user)
        {
            throw new NotImplementedException();
        }
        public async Task<Users?> GetUserByUsernameAsync(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty."); // Ném ngoại lệ nếu username không hợp lệ
            }
            return await _dbcontext.Users?.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public string GetMailBody(RegisterBaseModel model)
        {
            string apiUrl = _configuration["Host:https"];
            string token = GenerateVerificationToken(model.UserName);

            // Liên kết xác nhận sử dụng phương thức GET
            string url = $"{apiUrl}/api/Authen/confirm-email?token={token}";
            return string.Format(@"
    <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
        <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333;'>Welcome to <span style='color: #008CBA;'>Tutoring</span></h1>
            <p style='font-size: 16px; color: #555;'>Click the button below to verify your email and start your journey with us.</p>
            <a href='{0}' style='display: inline-block; 
                                 text-decoration: none;
                                 background-color: #008CBA;
                                 color: #ffffff;
                                 font-size: 18px;
                                 font-weight: bold;
                                 padding: 12px 24px;
                                 border-radius: 8px;
                                 box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                 transition: background-color 0.3s ease;'
               onmouseover='this.style.backgroundColor=""#0077A8""'
               onmouseout='this.style.backgroundColor=""#008CBA""'>
                Confirm Email
            </a>
            <p style='margin-top: 20px; font-size: 14px; color: #777;'>If you did not sign up for Tutoring, you can ignore this email.</p>
        </div>
    </div>", url);
        }
        public string GenerateVerificationToken(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, username),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTimeHelper.GetVietnamNow().AddMinutes(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
