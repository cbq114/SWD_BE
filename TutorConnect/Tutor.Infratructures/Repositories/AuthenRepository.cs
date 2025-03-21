using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
    public class AuthenRepository : IAuthenRepository
    {
        private readonly TutorDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly IWalletRepository _walletRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IMapper _mapper;

        public AuthenRepository(TutorDBContext context, IConfiguration configuration, IEmailSender emailSender, IWalletRepository walletRepository, IFavoriteRepository favoriteRepository, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
            _walletRepository = walletRepository;
            _favoriteRepository = favoriteRepository;
            _mapper = mapper;
        }
        public async Task<Users> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(email));
        }
        public async Task<string> GenerateRefreshToken(string username)
        {
            var refreshToken = Guid.NewGuid().ToString();
            var expriDate = DateTimeHelper.GetVietnamNow().AddDays(7);

            var newToken = new RefreshTokens
            {
                Token = refreshToken,
                UserName = username,
                Expires = expriDate,
            };
            _context.RefreshTokens.Add(newToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }
        public async Task<RefreshTokens> ValidateRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == token && rt.Expires > DateTimeHelper.GetVietnamNow());

            return refreshToken;
        }

        public async Task RevokeRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateAccessToken(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserName),
        new Claim(ClaimTypes.Role, user.Role.RoleName),
        new Claim("RoleId", user.RoleId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTimeHelper.GetVietnamNow().AddMinutes(300),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<Users> GetByUsername(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        }
        public async Task<(string, string)> LoginWithRefreshToken(string refreshToken)
        {
            var refreshTokenEntity = await ValidateRefreshToken(refreshToken);

            if (refreshTokenEntity == null)
                return (null, null);

            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.UserName == refreshTokenEntity.UserName && u.Status == UserStatus.verified)
                .SingleOrDefaultAsync();

            if (user == null)
                return (null, null);

            var accessToken = await GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshToken(user.UserName);

            return (accessToken, newRefreshToken);
        }
        public async Task<(string, string)> Login(LoginModel model)
        {
            var user = await _context.Users.Include(r => r.Role).AsNoTracking().Where(x => x.Status == UserStatus.verified).SingleOrDefaultAsync(x => x.Email.Equals(model.Email));
            if (user == null)
            {
                return (null, null);
            }
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return (null, null);
            }
            var accessToken = await GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshToken(user.UserName);

            return (accessToken, refreshToken);
        }

        public async Task<bool> Logout(string username)
        {
            return await Task.FromResult(true);
        }
        private async Task<string> ValidateBaseRegistration(RegisterBaseModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return "Email already exists";

            if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                return "Username already exists";

            if (await _context.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
                return "Phone number already exists";

            return null;
        }
        public async Task<string> RegisterInstructor(RegisterInstructorModel model)
        {
            var userAccountModel =  _mapper.Map<RegisterBaseModel>(model);
            var validationResult = await ValidateBaseRegistration(userAccountModel);
            if (validationResult != null)
                return validationResult;

            var cloudinaryService = new CloundinaryRepository(_configuration);
            string avatarUrl = null;
            if (model.Avatar != null)
                avatarUrl = await cloudinaryService.UploadImage(model.Avatar);

            string documentUrl = null;
            if (model.CVFile != null)
                documentUrl = await cloudinaryService.UploadImage(model.CVFile);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new Users
                {
                    UserName = model.UserName,
                    RoleId = 1, // Set tam role student
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName,
                    DOB = model.DOB,
                    CreatedDate = DateTimeHelper.GetVietnamNow(),
                    Avatar = avatarUrl ?? "Null avatar",
                    Status = UserStatus.pending
                };

                await _context.Users.AddAsync(user);

                var profile = new Domains.Entities.Profile
                {
                    UserName = model.UserName,
                    Address = model.Address,
                    Price = model.Price,
                    LanguageId = model.LanguageId,
                    TeachingExperience = model.TeachingExperience,
                    Country = model.Country,
                    Education = model.Education,
                    TutorStatus = TutorStatus.NotAvaliable
                };

                var request = new UpgradeRequest
                {
                    UserName = model.UserName,
                    DocumentUrl = documentUrl ?? "null docs",
                    Status = RequestStatus.Pending,
                    RequestedAt = DateTimeHelper.GetVietnamNow(),
                    reason = ""
                };

                await _context.Profile.AddAsync(profile);
                await _walletRepository.InitializeUserWallet(user.UserName);
                await _context.upgradeRequests.AddAsync(request);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                string emailBody = _emailSender.GetMailBody(model);
                bool emailSent = await _emailSender.EmailSendAsync(model.Email, "Instructor Account Created", emailBody);

                return emailSent ? "Please check email to verify your account." : "There was an error sending the email. Please try again later.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error while register tutor: {ex.Message}");
                return "Error while generate instructor account";
            }
        }

        public async Task<string> RegisterStudent(RegisterBaseModel model, IFormFile avatarFile)
        {
            var validationResult = await ValidateBaseRegistration(model);
            if (validationResult != null)
                return validationResult;

            string avatarUrl = null;
            if (avatarFile != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                avatarUrl = await
                    cloudinaryService.UploadImage(avatarFile);
            }

            var user = new Users
            {
                UserName = model.UserName,
                RoleId = 1, // Student role
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                DOB = model.DOB,
                CreatedDate = DateTimeHelper.GetVietnamNow().AddHours(7),
                Avatar = avatarUrl ?? "Null avatar",
                Status = UserStatus.pending
            };

            await _context.Users.AddAsync(user);
            await _walletRepository.InitializeUserWallet(user.UserName);
            await _favoriteRepository.InitializeFavoriteInstructors(user.UserName);

            await _context.SaveChangesAsync();

            string emailBody = _emailSender.GetMailBody(model);
            bool emailSent = await _emailSender.EmailSendAsync(model.Email, "Student Account Created", emailBody);

            return emailSent ? "Please check email to verify your account." : "There was an error sending the email. Please try again later.";
        }

        public string GenerateJwtToken(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Student" : user.RoleId == 2 ? "Tutor" : user.RoleId == 3 ? "Manager" : "Admin"),
                    new Claim("RoleId", user.RoleId.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim("Avatar", user.Avatar?.ToString() ?? string.Empty),
                };



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTimeHelper.GetVietnamNow().AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}