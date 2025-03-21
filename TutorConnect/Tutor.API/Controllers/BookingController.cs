using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDTO bookingDTO)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            bookingDTO.Customer = username;
            var bookingId = await _bookingService.CreateBooking(bookingDTO);

            switch (bookingId)
            {
                case -1:
                    return BadRequest(ApiResponse<string>.ErrorResult("Booking failed: Lesson not found"));
                case -2:
                    return BadRequest(ApiResponse<string>.ErrorResult("Booking failed: Tutor's schedule is unavailable or already booked"));
                case -3:
                    return BadRequest(ApiResponse<string>.ErrorResult("Booking failed: You need to complete the parent lesson first"));
                case -4:
                    return BadRequest(ApiResponse<string>.ErrorResult("Booking failed: Tutor not found"));
                case null:
                    return BadRequest(ApiResponse<string>.ErrorResult("Booking failed due to an unknown error"));
                default:
                    return Ok(ApiResponse<object>.SuccessResult(new { bookingId }));
            }
        }

        [HttpGet("get-all-of-user")]
        [Authorize]
        public async Task<IActionResult> GetAllBookingOfUser()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var result = await _bookingService.GetAllBookingOfUser(username);

            if (!result.Any())
                return NotFound(ApiResponse<string>.ErrorResult($"Can not found any booking of user: {username}"));

            return Ok(ApiResponse<List<BookingDTO>>.SuccessResult(result));
        }

        [HttpGet("get-all-of-tutor")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> GetAllBookingOfTutor()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return Unauthorized(ApiResponse<string>.ErrorResult("Unauthorized"));

            var result = await _bookingService.GetAllBookingTutor(username);

            if (!result.Any())
                return NotFound(ApiResponse<string>.ErrorResult($"Can not found any booking of tutor: {username}"));

            return Ok(ApiResponse<List<BookingDTO>>.SuccessResult(result));
        }

        [HttpGet("get-by-id/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var booking = await _bookingService.GetBookingById(bookingId);

            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Can not found any booking by this id: {bookingId}"));

            return Ok(ApiResponse<BookingDTO>.SuccessResult(booking));
        }

        [HttpPut("cancel/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            var booking = await _bookingService.GetBookingById(bookingId);

            if (booking == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Can not found any booking by this id: {bookingId}"));

            var result = await _bookingService.CancelBooking(bookingId);
            if (result)
                return BadRequest(ApiResponse<string>.ErrorResult("This booking can not be cancel!"));
            return Ok(ApiResponse<string>.SuccessResult("Cancel Succesfully"));
        }
    }
}
