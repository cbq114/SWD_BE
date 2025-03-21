using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<(bool success, string message, Users? user)> InitiateForgotPasswordAsync(string email)
        {
            return await _userRepository.InitiateForgotPasswordAsync(email);
        }

        public async Task<(bool success, string message)> VerifyCodeAsync(string email, string code)
        {
            return await _userRepository.VerifyCodeAsync(email, code);
        }

        public async Task<(bool success, string message)> ResetPasswordAsync(string email, string newPassword)
        {
            return await _userRepository.ResetPasswordAsync(email, newPassword);
        }

        public Task<UserViewModel> GetByEmailAsync(string email)
        {
            return _userRepository.GetByEmailAsync(email);
        }

        public Task<UserViewModel?> GetByUsernameAsync(string username)
        {
            return _userRepository.GetByUsernameAsync(username);
        }

        public Task<UserViewInstructorModel> UserViewInstructorModel(string username)
        {
            return _userRepository.UserViewInstructorModel(username);
        }

        public Task<UserViewInstructorModel> GetUserViewInstructorModelByEmail(string email)
        {
            return _userRepository.GetUserViewInstructorModelByEmail(email);
        }
        public async Task<List<UserViewInstructorModel>> GetAllInstructors()
        {
            return await _userRepository.GetAllInstructors();
        }
        public async Task<List<UserViewModel>> GetAllStudents()
        {
            return await _userRepository.GetAllStudents();
        }
        public async Task<List<UserViewInstructorModel>> GetFilteredInstructors(SearchOptionUser searchOption)
        {
            return await _userRepository.GetFilteredInstructors(searchOption);
        }

        public async Task<string> UpdateTutorProfile(string username, UpdateTutor updateTutor)
        {
            var user = await _userRepository.GetUserByUserName(username);
            if (user == null)
                return "User does not exist";

            _mapper.Map(updateTutor, user);

            await _userRepository.UpdateTutorProfile(user);

            return "Update successful";
        }

        public async Task<string> UpdateUserProfile(string username, UpdateUser updateUser)
        {
            var user = await _userRepository.GetUserByUserName(username);
            if (user == null)
                return "User does not exist";

            _mapper.Map(updateUser, user);

            await _userRepository.UpdateUserProfile(user);

            return "Update successful";
        }

        public async Task<string> UpdateAvatar(string userName, IFormFile avatarFile)
        {
            return await _userRepository.UpdateAvatar(userName, avatarFile);
        }

        public async Task<Users> GetCurrentUser(string username)
        {
            return await _userRepository.GetCurrentUser(username);
        }

        public Task<Users> GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }

        public Task<Users> CreateUser(Users user)
        {
            return _userRepository.CreateUser(user);
        }

        public Task<bool> CheckFavoriteInstructor(string userName)
        {
            return _userRepository.CheckFavoriteInstructor(userName);
        }

        public Task<bool> CheckWalletExists(string userName)
        {
            return _userRepository.CheckWalletExists(userName);
        }

        public Task<string> GenerateJWTTOKEN(Users user)
        {
            return _userRepository.GenerateJWTTOKEN(user);
        }

        public async Task<Users?> GetTargetUserInRoom(int roomId, string currentUsername)
        {
            //var usersInRoom = await _userRepository.GetUsersInRoom(roomId);
            //return usersInRoom.FirstOrDefault(user => user.UserName != currentUsername);
            return await _userRepository.GetTargetUserInRoom(roomId, currentUsername);
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            return await _userRepository.DeleteUserAsync(username);
        }

        public async Task<List<UserModelDTO>> GetAllAccount(UserFilter userFilter)
        {
            return await _userRepository.GetAllAccount(userFilter);
        }

        public async Task<string> BanAccount(string username)
        {
            if (username.IsNullOrEmpty())
                return "Error: Invaid user name";

            var user = await _userRepository.GetCurrentUser(username);
            if (user.Status == Domains.Enums.UserStatus.blocked)
                return "Error: This account is already banned.";

            var resut = await _userRepository.UpdateAccountStatus(username, Domains.Enums.UserStatus.blocked);
            if (resut)
                return "Account ban successfully";
            return "Error: There was an error while ban account, please try later.";
        }

        public async Task<string> UpdateRole(string username, int rolesId)
        {
            var role = await _roleRepository.GetRolesById(rolesId);
            if (role == null)
                return $"Error: Can not find role with id: {rolesId}";

            var result = await _userRepository.UpdateAccountRole(username, rolesId);
            if (result)
                return "Update successfully.";

            return "Error while update role!";
        }

        public async Task<bool> BlockAccount(string username)
        {
            return await _userRepository.BlockAccount(username);
        }
        public async Task<bool> UnBlockAccount(string username)
        {
            return await _userRepository.UnBlockAccount(username);
        }
    }
}