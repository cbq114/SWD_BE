using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Shared.Helper;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userServices;
        private readonly IAuthenService _authenServices;

        public GoogleAuthController(IConfiguration configuration, IUserService userService, IAuthenService authenService)
        {
            _configuration = configuration;
            _userServices = userService;
            _authenServices = authenService;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("Authentication failed.");

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var avatar = authenticateResult.Principal.FindFirst("picture")?.Value;
            var phone = authenticateResult.Principal.FindFirst(ClaimTypes.MobilePhone)?.Value;



            var user = await _userServices.GetUserByEmail(email);

            if (user == null)
            {
                var encodedName = Uri.EscapeDataString(name);
                var encodedEmail = Uri.EscapeDataString(email);
                var encodedPhone = Uri.EscapeDataString(phone ?? "Unknown");
                var encodedAvatar = Uri.EscapeDataString(avatar ?? "Unknown");
                return Redirect($"http://localhost:5173/auth/select-role?email={encodedEmail}&name={encodedName}&avatar={encodedAvatar}&phone={encodedPhone}");
            }
            string token = await _userServices.GenerateJWTTOKEN(user);

            return Redirect($"http://localhost:5173/auth/login-success?token={token}");
        }

        [HttpPost("select-role-user")]
        public async Task<IActionResult> SelectRole([FromQuery] string email, [FromQuery] string name, [FromQuery] string avatar, [FromQuery] string phone, [FromBody] int roleId)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Missing email or name.");
            }

            var existingUser = await _userServices.GetUserByEmail(email);
            if (existingUser != null)
            {
                return BadRequest("User already exists.");
            }

            UserStatus status;
            if (roleId == 1)
            {
                status = UserStatus.verified;
            }
            else if (roleId == 2)
            {
                status = UserStatus.pending;
            }
            else
            {
                return BadRequest("Invalid role ID.");
            }

            var user = new Users
            {
                UserName = email,
                Email = email,
                FullName = name,
                Password = GenerateRandomPassword(),
                PhoneNumber = phone,
                CreatedDate = DateTimeHelper.GetVietnamNow(),
                DOB = DateTimeHelper.GetVietnamNow(),
                Status = status,
                RoleId = roleId,
                Avatar = avatar
            };

            await _userServices.CreateUser(user);

            string token = await _userServices.GenerateJWTTOKEN(user);
            return Ok(new { message = "User created successfully", token });
        }

        private string GenerateRandomPassword()
        {

            return "defaultPassword";
        }
    }
}
