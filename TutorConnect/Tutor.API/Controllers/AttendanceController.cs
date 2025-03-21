using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IBookingService _bookingService;

        public AttendanceController(IAttendanceService attendanceService, IBookingService bookingService)
        {
            _attendanceService = attendanceService;
            _bookingService = bookingService;
        }

        [HttpPost("start-lesson/{bookingId}")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> StartLesson(int bookingId)
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var booking = await _bookingService.GetBookingById(bookingId);
            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Booking not found with id: {bookingId}"));

            if (booking.Instructor != username)
                return Unauthorized(ApiResponse<string>.ErrorResult("You are not authorized to start this lesson"));

            var result = await _attendanceService.StartLesson(bookingId);
            if (!result)
                return BadRequest(ApiResponse<string>.ErrorResult("Failed to start lesson. Check if lesson is scheduled for now"));

            return Ok(ApiResponse<string>.SuccessResult("Lesson started successfully"));
        }

        [HttpPost("mark-student-attendance")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> MarkStudentAttendance([FromBody] MarkAttendanceDTO attendanceDTO)
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var booking = await _bookingService.GetBookingById(attendanceDTO.BookingId);
            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Booking not found with id: {attendanceDTO.BookingId}"));

            if (booking.Instructor != username)
                return Unauthorized(ApiResponse<string>.ErrorResult("You are not authorized to mark attendance for this lesson"));

            var result = await _attendanceService.MarkAttendance(attendanceDTO);
            if (!result)
                return BadRequest(ApiResponse<string>.ErrorResult("Failed to mark attendance"));

            return Ok(ApiResponse<string>.SuccessResult("Attendance marked successfully"));
        }

        [HttpPost("end-lesson/{bookingId}")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> EndLesson(int bookingId, [FromBody] LessonCompletionDTO completionDTO)
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var booking = await _bookingService.GetBookingById(bookingId);
            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Booking not found with id: {bookingId}"));

            if (booking.Instructor != username)
                return Unauthorized(ApiResponse<string>.ErrorResult("You are not authorized to end this lesson"));

            var result = await _attendanceService.EndLesson(bookingId, completionDTO);
            if (!result)
                return BadRequest(ApiResponse<string>.ErrorResult("Failed to end lesson"));

            return Ok(ApiResponse<string>.SuccessResult("Lesson ended successfully"));
        }

        [HttpGet("student-join/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> StudentJoinLesson(int bookingId)
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var booking = await _bookingService.GetBookingById(bookingId);
            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Booking not found with id: {bookingId}"));

            if (booking.customer != username)
                return Unauthorized(ApiResponse<string>.ErrorResult("You are not authorized to join this lesson"));

            var meetLink = await _attendanceService.StudentJoinLesson(bookingId);
            if (string.IsNullOrEmpty(meetLink))
                return BadRequest(ApiResponse<string>.ErrorResult("Failed to join lesson. Lesson may not have started yet"));

            return Ok(ApiResponse<string>.SuccessResult(meetLink, "Successfully joined lesson"));
        }

        [HttpGet("lesson-history/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetLessonHistory(int bookingId)
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var booking = await _bookingService.GetBookingById(bookingId);
            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Booking not found with id: {bookingId}"));

            if (booking.customer != username && booking.Instructor != username)
                return Unauthorized(ApiResponse<string>.ErrorResult("You are not authorized to view this lesson history"));

            var result = await _attendanceService.GetLessonAttendanceHistory(bookingId);
            if (result == null)
                return NotFound(ApiResponse<string>.ErrorResult("No lesson history found"));

            return Ok(ApiResponse<LessonAttendanceHistoryDTO>.SuccessResult(result));
        }

        [HttpGet("get-active-lessons")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> GetActiveLessons()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var result = await _attendanceService.GetActiveLessons(username);
            return Ok(ApiResponse<List<ActiveLessonDTO>>.SuccessResult(result));
        }
    }
}
