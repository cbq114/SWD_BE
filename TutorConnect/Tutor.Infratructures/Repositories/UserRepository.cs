using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.UserModel;
using Tutor.Infratructures.Persistence;
using Tutor.Shared.Helper;

namespace Tutor.Infratructures.Repositories
{
    public class UserRepository : Repository<Users>, IUserRepository
    {
        private readonly TutorDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private const int VERIFICATION_CODE_LENGTH = 6;
        private const int VERIFICATION_CODE_EXPIRY_MINUTES = 15;
        private readonly IMapper _mapper;
        private readonly IAuthenRepository _authen;


        public UserRepository(TutorDBContext dbContext, IMapper mapper, IAuthenRepository authenRepository, IConfiguration configuration) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authen = authenRepository;
            _configuration = configuration;
        }

        public async Task<(bool success, string message, Users? user)> InitiateForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return (false, "No account found with this email address.", null);
                }

                string verificationCode = GenerateVerificationCode();
                user.VerificationCode = verificationCode;
                user.VerificationCodeExpiry = DateTimeHelper.GetVietnamNow().AddMinutes(VERIFICATION_CODE_EXPIRY_MINUTES);

                await _dbContext.SaveChangesAsync();

                return (true, "Verification code generated successfully.", user);
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while processing your request.", null);
            }
        }

        public async Task<(bool success, string message)> VerifyCodeAsync(string email, string code)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return (false, "Invalid verification attempt.");
                }

                if (string.IsNullOrEmpty(user.VerificationCode) ||
                    user.VerificationCodeExpiry < DateTimeHelper.GetVietnamNow())
                {
                    return (false, "Verification code has expired.");
                }

                if (user.VerificationCode != code)
                {
                    return (false, "Invalid verification code.");
                }

                return (true, "Code verified successfully.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while verifying the code.");
            }
        }

        public async Task<(bool success, string message)> ResetPasswordAsync(string email, string newPassword)
        {
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return (false, "Invalid reset password attempt.");
                }

                if (string.IsNullOrEmpty(user.VerificationCode) ||
                    user.VerificationCodeExpiry < DateTimeHelper.GetVietnamNow())
                {
                    return (false, "Reset password session has expired.");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.VerificationCode = null;
                user.VerificationCodeExpiry = null;

                await _dbContext.SaveChangesAsync();

                return (true, "Password has been reset successfully.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while resetting the password.");
            }
        }

        private string GenerateVerificationCode()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                uint random = BitConverter.ToUInt32(bytes, 0) % 1000000;
                return random.ToString("D6");
            }
        }

        public async Task<UserViewModel> GetByEmailAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Status == UserStatus.verified);
            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewModel?> GetByUsernameAsync(string username)
        {
            var user = await Entities.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewInstructorModel> UserViewInstructorModel(string username)
        {
            var user = await _dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower() && u.Status == UserStatus.verified);
            return _mapper.Map<UserViewInstructorModel>(user);
        }


        public async Task<UserViewInstructorModel> GetUserViewInstructorModelByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Status == UserStatus.verified);
            return _mapper.Map<UserViewInstructorModel>(user);
        }

        public async Task<List<UserViewInstructorModel>> GetAllInstructors()
        {
            var instructors = await _dbContext.Users.Include(u => u.Role).Include(u => u.Profile).Where(u => u.Role.RoleName == "Tutor" && u.Status == UserStatus.verified).ToListAsync();
            return _mapper.Map<List<UserViewInstructorModel>>(instructors);
        }

        public async Task<List<UserViewModel>> GetAllStudents()
        {
            var students = await _dbContext.Users.Include(u => u.Role).Include(u => u.Profile).Where(u => u.Role.RoleName == "Student" && u.Status == UserStatus.verified).ToListAsync();
            return _mapper.Map<List<UserViewModel>>(students);
        }
        public async Task<Users> CreateUser(Users user)
        {
            await Entities.AddAsync(user);
            return user;
        }
        public async Task<Users> GetUserByEmail(string email)
        {
            return await Entities.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<UserViewInstructorModel>> GetFilteredInstructors(SearchOptionUser searchOption)
        {
            IQueryable<Users> query = _dbContext.Users
                .Include(u => u.Profile)
                .Include(u => u.Languagues)
                .Where(u => u.Role.RoleName == "Tutor" && u.Status == UserStatus.verified);

            // Apply filters based on search options
            if (!string.IsNullOrWhiteSpace(searchOption.FullName))
            {
                query = query.Where(u => u.FullName.Contains(searchOption.FullName));
            }

            if (!string.IsNullOrWhiteSpace(searchOption.Country))
            {
                query = query.Where(u => u.Profile.Country.Contains(searchOption.Country));
            }

            if (searchOption.TutorStatus.HasValue)
            {
                query = query.Where(u => u.Profile.TutorStatus == searchOption.TutorStatus);
            }

            if (!string.IsNullOrWhiteSpace(searchOption.LanguageName))
            {
                query = query.Where(u => u.Languagues.Any(s => s.LanguageName.Contains(searchOption.LanguageName)));
            }

            // Execute query and map to view model
            List<Users> instructors = await query.ToListAsync();
            return _mapper.Map<List<UserViewInstructorModel>>(instructors);
        }

        public async Task<Users> GetUserByUserName(string userName)
        {
            return await _dbContext.Users
                .Include(u => u.Profile)
                .ThenInclude(p => p.Certifications)
                .SingleOrDefaultAsync(u => u.UserName == userName && u.Status == UserStatus.verified);
        }
        public async Task<Users> GetUserByUserNameAll(string userName)
        {
            return await _dbContext.Users
                .Include(u => u.Profile)
                .ThenInclude(p => p.Certifications)
                .SingleOrDefaultAsync(u => u.UserName == userName);
        }
        public async Task<string> UpdateTutorProfile(Users updateTutor)
        {
            await base.Update(updateTutor);

            return "Updated success";
        }

        public async Task<string> UpdateUserProfile(Users user)
        {
            await base.Update(user);

            return "Updated successfully";
        }
        public async Task<string> UpdateAvatar(string usnerName, IFormFile avatarFile)
        {
            Users user = await GetUserByUserName(usnerName);
            if (user == null)
            {
                return "User does not exist";
            }

            string avatarUrl = null;
            if (avatarFile != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                avatarUrl = await cloudinaryService.UploadImage(avatarFile);
            }

            user.Avatar = avatarUrl;
            await base.Update(user);
            return " Updated successfully";
        }

        public async Task<Users> GetCurrentUser(string username)
        {
            var user = await _dbContext.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower() && u.Status == UserStatus.verified);
            return user;
        }

        public async Task<bool> CheckFavoriteInstructor(string userName)
        {

            return await _dbContext.FavoriteInstructors.AnyAsync(b => b.UserName == userName);
        }
        public async Task<bool> CheckWalletExists(string userName)
        {
            return await _dbContext.Wallet.AnyAsync(w => w.UserName == userName);
        }
        public async Task<string> GenerateJWTTOKEN(Users user)
        {

            if (user.FavoriteInstructors == null && !await CheckFavoriteInstructor(user.UserName))
            {
                var favoriteInstructor = new FavoriteInstructors
                {
                    UserName = user.UserName,
                };
                user.FavoriteInstructors = favoriteInstructor;
                _dbContext.FavoriteInstructors.Add(favoriteInstructor);
                await _dbContext.SaveChangesAsync();

            }

            if (user.Wallet == null && !await CheckWalletExists(user.UserName))
            {
                var wallet = new Wallet
                {
                    UserName = user.UserName,
                    Balance = 1,
                    TransactionTime = DateTimeHelper.GetVietnamNow(),
                };
                user.Wallet = wallet;
                _dbContext.Wallet.Add(wallet);
                await _dbContext.SaveChangesAsync();

            }
            //----------------------------------------------------------------------
            var token = _authen.GenerateJwtToken(user);
            return token;
        }
        public async Task<List<Users>> GetUsersInRoom(int roomId)
        {
            return await _dbContext.MessageRooms
                .Where(room => room.Id == roomId)
                .SelectMany(room => room.Members)
                .Select(member => member.User)
                .ToListAsync();
        }
        public async Task<Users?> GetTargetUserInRoom(int roomId, string currentUsername)
        {
            return await _dbContext.MessageRooms
                .Where(room => room.Id == roomId)
                .SelectMany(room => room.Members)
                .Where(member => member.User.UserName != currentUsername)
                .Select(member => member.User)
                .FirstOrDefaultAsync();
        }

        //Get admin account
        public async Task<Users> GetAdmin()
        {
            var admin = await Entities.FirstOrDefaultAsync(u => u.RoleId == 4);
            return admin;
        }

        public async Task<Users> GetTutor(string username)
        {
            return await Entities.FirstOrDefaultAsync(u => u.UserName == username && u.RoleId == 2);
        }


        //Dành cho dasboard admin

        public async Task<bool> DeleteUserAsync(string username)
        {
            var user = await Entities.FindAsync(username);
            if (user == null) return false;
            Entities.Remove(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<UserModelDTO>> GetAllAccount(UserFilter userFilter)
        {
            var usersQuery = Entities.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(userFilter?.UserName))
            {
                usersQuery = usersQuery.Where(u => u.UserName.Contains(userFilter.UserName));
            }


            if (!string.IsNullOrEmpty(userFilter?.Email))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(userFilter.Email));
            }

            if (!string.IsNullOrEmpty(userFilter?.PhoneNumber))
            {
                usersQuery = usersQuery.Where(u => u.PhoneNumber.Contains(userFilter.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(userFilter?.FullName))
            {
                usersQuery = usersQuery.Where(u => u.FullName.Contains(userFilter.FullName));
            }

            if (userFilter?.Status.HasValue == true)
            {
                usersQuery = usersQuery.Where(u => u.Status == userFilter.Status);
            }

            var users = await usersQuery.ToListAsync();
            return _mapper.Map<List<UserModelDTO>>(users);
        }

        public async Task<bool> UpdateAccountStatus(string username, UserStatus status)
        {
            var user = await Entities.FindAsync(username);
            if (user == null)
                return false;

            user.Status = status;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAccountRole(string username, int roleId)
        {
            var user = await Entities.FindAsync(username);
            if (user == null) return false;

            user.RoleId = roleId;
            Entities.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public async Task<bool> BlockAccount(string username)
        {
            var user = await GetUserByUserName(username);
            if (user == null) return false;
            else
            {
                user.Status = UserStatus.blocked;
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> UnBlockAccount(string username)
        {
            var user = await GetUserByUserNameAll(username);
            if (user == null) return false;
            else
            {
                user.Status = UserStatus.verified;
                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}