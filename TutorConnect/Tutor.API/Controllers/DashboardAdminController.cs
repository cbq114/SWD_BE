using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.Responses;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardAdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDashboardService _service;
        private readonly IRoleService _roleService;

        public DashboardAdminController(IUserService userService, IDashboardService dashboardService, IRoleService roleService)
        {
            _userService = userService;
            _service = dashboardService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilter userFilter)
        {
            var users = await _userService.GetAllAccount(userFilter);
            return Ok(ApiResponse<List<UserModelDTO>>.SuccessResult(users));
        }

        [HttpGet("get-user-by/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userService.GetByUsernameAsync(username);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        [HttpGet("get-roles")]
        public async Task<IActionResult> GetUser()
        {
            var roles = await _roleService.GetAllRoles();
            if (!roles.Any()) return NotFound(ApiResponse<string>.ErrorResult("Can not found any role"));
            return Ok(ApiResponse<List<RoleDTO>>.SuccessResult(roles));
        }

        [HttpPut("update-user-role")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserRoleRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username))
                return NotFound(ApiResponse<string>.ErrorResult("User not found"));

            if (request.RoleId <= 0)
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid request data"));

            try
            {
                string result = await _userService.UpdateRole(request.Username, request.RoleId);

                if (result.Contains("Error"))
                    return NotFound(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while updating the Account role: " + ex.Message));
            }
        }

        [HttpPut("ban-account")]
        public async Task<IActionResult> BanAccount([FromBody] string username)
        {
            try
            {
                var result = await _userService.BanAccount(username);
                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while ban the account: " + ex.Message));
            }
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings()
    => Ok(await _service.GetBookingsAsync());

        [HttpGet("bookings/{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
            => Ok(await _service.GetBookingByIdAsync(bookingId));

        [HttpPut("bookings/{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, [FromBody] UpdateBookingStatusRequest request)
        {
            await _service.UpdateBookingStatusAsync(bookingId, request.Status);
            return Ok(new { message = "Updated successfully." });
        }

        [HttpDelete("bookings/{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            await _service.DeleteBookingAsync(bookingId);
            return Ok(new { message = "Deleted successfully." });
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
            => Ok(await _service.GetPaymentsAsync());

        [HttpGet("refunds")]
        public async Task<IActionResult> GetRefunds()
            => Ok(await _service.GetRefundsAsync());

        [HttpGet("refunds/{refundId}")]
        public async Task<IActionResult> GetRefundById(int refundId)
            => Ok(await _service.GetRefundByIdAsync(refundId));

        [HttpGet("payouts")]
        public async Task<IActionResult> GetPayouts()
            => Ok(await _service.GetPayoutsAsync());

        [HttpGet("feedbacks")]
        public async Task<IActionResult> GetFeedbacks()
            => Ok(await _service.GetFeedbacksAsync());

        [HttpDelete("feedbacks/{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            await _service.DeleteFeedbackAsync(feedbackId);
            return Ok(new { message = "Feedback deleted successfully." });
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var data = await _service.GetDashboardSummaryAsync();
            return Ok(data);
        }

        // API: /api/DashboardAdmin/revenue-monthly?year=2025
        [HttpGet("revenue-monthly")]
        public async Task<IActionResult> GetMonthlyRevenue(int year)
        {
            var data = await _service.GetMonthlyRevenueAsync(year);
            return Ok(data);
        }

        // API: /api/DashboardAdmin/user-distribution
        [HttpGet("user-distribution")]
        public async Task<IActionResult> GetUserDistribution()
        {
            var data = await _service.GetUserDistributionAsync();
            return Ok(data);
        }

        // API: /api/DashboardAdmin/user-growth?year=2025
        [HttpGet("user-growth")]
        public async Task<IActionResult> GetUserGrowth(int year)
        {
            var data = await _service.GetUserGrowthAsync(year);
            return Ok(data);
        }
        public class UpdateBookingStatusRequest
        {
            [Required(ErrorMessage = "Booking status is required")]
            [EnumDataType(typeof(BookingStatus), ErrorMessage = "Invalid booking status.")]
            public string Status { get; set; }
        }
    }
    public class UpdateUserRoleRequest
    {
        public string Username { get; set; }
        public int RoleId { get; set; }
    }

}
