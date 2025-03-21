using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Responses;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public UserController(IUserService userService, IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var (success, message, user) = await _userService.InitiateForgotPasswordAsync(request.Email);
            if (!success || user == null)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(message));
            }

            var emailSent = await _emailSender.EmailSendAsync(
                request.Email,
                "Password Reset Code",
                GetPasswordResetEmailTemplate(user.VerificationCode)
            );

            if (!emailSent)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Failed to send verification code."));
            }

            return Ok(ApiResponse<string>.SuccessResult("Verification code has been sent to your email."));
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var (success, message) = await _userService.VerifyCodeAsync(request.Email, request.Code);
            if (!success)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(message));
            }
            return Ok(ApiResponse<string>.SuccessResult(message));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var (success, message) = await _userService.ResetPasswordAsync(request.Email, request.NewPassword);
            if (!success)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(message));
            }
            return Ok(ApiResponse<string>.SuccessResult(message));
        }

        //[Authorize(Roles = "Tutor")]
        [HttpGet("instructor")]
        public async Task<IActionResult> GetUserViewInstructorModel()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.UserViewInstructorModel(username);
            if (user == null)
            {
                return NotFound(ApiResponse<UserViewInstructorModel>.ErrorResult("User not found"));
            }
            return Ok(ApiResponse<UserViewInstructorModel>.SuccessResult(user));
        }

        [HttpGet("/instructor/{username}")]
        public async Task<IActionResult> GetUserViewInstructorModelByUserName(string username)
        {
            var user = await _userService.UserViewInstructorModel(username);
            if (user == null)
            {
                return NotFound(ApiResponse<UserViewInstructorModel>.ErrorResult("User not found"));
            }
            return Ok(ApiResponse<UserViewInstructorModel>.SuccessResult(user));
        }

        [Authorize(Roles = "Tutor")]
        [HttpGet("instructor/email")]
        public async Task<IActionResult> GetUserViewInstructorModelByEmail()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userService.GetUserViewInstructorModelByEmail(email);
            if (user == null)
            {
                return NotFound(ApiResponse<UserViewInstructorModel>.ErrorResult("User not found"));
            }
            return Ok(ApiResponse<UserViewInstructorModel>.SuccessResult(user));
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {

            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound(ApiResponse<UserViewModel>.ErrorResult("User not found"));
            }
            return Ok(ApiResponse<UserViewModel>.SuccessResult(user));
        }

        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _userService.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(ApiResponse<UserViewModel>.ErrorResult("User not found"));
            }
            return Ok(ApiResponse<UserViewModel>.SuccessResult(user));
        }

        [HttpGet("get-all-instructor")]
        public async Task<IActionResult> GetAllInstructors()
        {
            var instructors = await _userService.GetAllInstructors();
            return Ok(ApiResponse<List<UserViewInstructorModel>>.SuccessResult(instructors));
        }

        [HttpGet("get-all-students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _userService.GetAllStudents();
            return Ok(ApiResponse<List<UserViewModel>>.SuccessResult(students));
        }

        [HttpGet("FilterInstructor")]
        public async Task<IActionResult> GetFilteredInstructors([FromQuery] SearchOptionUser searchOption)
        {
            var instructors = await _userService.GetFilteredInstructors(searchOption);
            return Ok(ApiResponse<List<UserViewInstructorModel>>.SuccessResult(instructors));
        }

        [HttpPut("update_user_profile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUser updateUser)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return NotFound(ApiResponse<string>.ErrorResult("User not found"));
            }
            if (updateUser == null)
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid request data"));

            try
            {
                string result = await _userService.UpdateUserProfile(username, updateUser);

                if (result == "User does not exist")
                    return NotFound(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while updating the profile: " + ex.Message));
            }
        }

        [Authorize(Roles = "Tutor")]
        [HttpPut("update_tutor_profile")]
        public async Task<IActionResult> UpdateTutorProfile([FromForm] UpdateTutor updateTutor)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
            {
                return NotFound(ApiResponse<string>.ErrorResult("User not found"));
            }
            if (updateTutor == null)
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid request data"));

            try
            {
                string result = await _userService.UpdateTutorProfile(username, updateTutor);

                if (result == "User does not exist")
                    return NotFound(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while updating the profile: " + ex.Message));
            }
        }

        [HttpPut("update_avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Username is required."));
            }

            if (avatarFile == null || avatarFile.Length == 0)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Avatar file is required."));
            }

            try
            {
                var result = await _userService.UpdateAvatar(userName, avatarFile);

                if (result.Contains("does not exist"))
                {
                    return NotFound(ApiResponse<string>.ErrorResult(result));
                }

                return Ok(ApiResponse<string>.SuccessResult(result, "Avatar updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult($"An error occurred while updating the avatar: {ex.Message}"));
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Block_account/{username}")]
        public async Task<IActionResult> BlockAccount(string username)
        {
            if (username.IsNullOrEmpty())
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid username"));
            var result = await _userService.BlockAccount(username);
            if (!result)
                return BadRequest(ApiResponse<string>.ErrorResult("Error while block account"));
            return Ok(ApiResponse<string>.SuccessResult("Block success"));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UnBlock_account/{username}")]
        public async Task<IActionResult> UnBlockAccount(string username)
        {
            if (username.IsNullOrEmpty())
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid username"));
            var result = await _userService.UnBlockAccount(username);
            if (!result)
                return BadRequest(ApiResponse<string>.ErrorResult("Error while un block account"));
            return Ok(ApiResponse<string>.SuccessResult("UnBlock success"));
        }
        private string GetPasswordResetEmailTemplate(string code)
        {
            return $@"
                <div style='text-align: center; font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                        <h1 style='color: #333;'>Password Reset Request</h1>
                        <p style='font-size: 16px; color: #555;'>Your verification code is:</p>
                        <div style='margin: 25px 0; padding: 15px; background-color: #f5f5f5; border-radius: 5px;'>
                            <span style='font-size: 24px; font-weight: bold; letter-spacing: 3px; color: #008CBA;'>{code}</span>
                        </div>
                        <p style='font-size: 14px; color: #777;'>This code will expire in 15 minutes.</p>
                        <p style='font-size: 14px; color: #777;'>If you didn't request this password reset, please ignore this email.</p>
                    </div>
                </div>";
        }
    }

}