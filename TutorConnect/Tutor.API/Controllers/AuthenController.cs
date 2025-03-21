﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenService _service;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthenController> _logger;

        public AuthenController(IConfiguration configuration, IUnitOfWork unitOfWork, IAuthenService service, IEmailSender emailSender, ILogger<AuthenController> logger)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _service = service;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { code = 400, message = "Invalid username or password" });
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var (accessToken, refreshToken) = await _service.Login(model);

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new { code = 401, message = "Invalid username or password" });
                }

                await _unitOfWork.CommitAsync();
                return Ok(new
                {
                    code = 200,
                    accessToken,
                    refreshToken,
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { code = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { code = 400, message = "Refresh token is required" });

            try
            {
                var (accessToken, newRefreshToken) = await _service.LoginWithRefreshToken(refreshToken);

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(newRefreshToken))
                    return Unauthorized(new { code = 401, message = "Invalid refresh token" });

                return Ok(new
                {
                    code = 200,
                    accessToken,
                    refreshToken = newRefreshToken,
                    message = "Token refreshed successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { code = 500, message = ex.Message });
            }
        }

        [HttpPost("register/student")]
        public async Task<ActionResult<ApiResponse<string>>> RegisterStudent([FromForm] RegisterBaseModel model, IFormFile avatarFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid input data"));
            }
            var result = await _service.RegisterStudent(model, avatarFile);

            if (result.Contains("exists"))
            {
                return BadRequest(ApiResponse<string>.ErrorResult(result));
            }

            return Ok(ApiResponse<string>.SuccessResult(result));
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid token.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var username = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                var result = await _emailSender.ConfirmEmailAsync(username);
                return Content(@"
    <html>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Account Confirmed</title>
            <script type='text/javascript'>
                window.onload = function() {
                    alert('Your account has been successfully confirmed.');
                };
            </script>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    text-align: center;
                    background-color: #f4f4f4;
                    padding-top: 50px;
                }
                .container {
                    background: #ffffff;
                    padding: 30px;
                    border-radius: 10px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                    max-width: 400px;
                    margin: auto;
                }
                .notification {
                    font-size: 22px;
                    color: #28a745;
                    font-weight: bold;
                    margin-bottom: 20px;
                }
                .btn {
                    display: inline-block;
                    padding: 12px 24px;
                    font-size: 16px;
                    color: #ffffff;
                    background-color: #28a745;
                    border: none;
                    border-radius: 8px;
                    text-decoration: none;
                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    transition: background-color 0.3s ease;
                }
                .btn:hover {
                    background-color: #218838;
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='notification'>Your account has been successfully confirmed.</div>
                <a href='https://tutoring.id.vn/auth/login' class='btn'>Go to Login</a>
            </div>
        </body>
    </html>", "text/html");
            }
            catch (SecurityTokenExpiredException)
            {
                return BadRequest("Token has expired.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { message = "Username cannot be null or empty." });
            }

            try
            {
                var user = await _emailSender.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                if (user.Status == UserStatus.verified)
                {
                    return Ok("Email have been verified");
                }

                string emailBody = _emailSender.GetMailBody(new RegisterBaseModel
                {
                    UserName = user.UserName,
                    Email = user.Email
                });

                bool emailSent = await _emailSender.EmailSendAsync(user.Email, "Verify your account", emailBody);

                if (!emailSent)
                {
                    return StatusCode(500, "Failed to send verification email.");
                }

                return Ok("Pls check email to verify.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("register/instructor")]
        public async Task<ActionResult<ApiResponse<string>>> RegisterInstructor([FromForm] RegisterInstructorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid input data"));
            }
            var result = await _service.RegisterInstructor(model);

            if (result.Contains("exists"))
            {
                return BadRequest(ApiResponse<string>.ErrorResult(result));
            }

            return Ok(ApiResponse<string>.SuccessResult(result));
        }
    }
}
