using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Applications.Services;
using Tutor.Infratructures.Models.FavoriteModel;
using Tutor.Infratructures.Models.Responses;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IBookingService _bookingService;

        public FeedbacksController(IFeedbackService feedbackService, IBookingService bookingService) 
        { 
            _feedbackService = feedbackService;
            _bookingService = bookingService;
        }

        [HttpGet("get-tutor-feedback")]
        public async Task<ActionResult<List<FeedbacksDTO>>> LoadAllFeedbackOfTutor([FromQuery]string tutorname, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(tutorname))
                return BadRequest("Tuor username cannot be null");

            var feedbacks = await _feedbackService.GetAllFeedbacksOf(tutorname);
            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound(ApiResponse<string>.ErrorResult($"{tutorname} dont have any feedback"));
            }
            var PageList = feedbacks.ToPagedList(pageNumber, pageSize);

            return Ok(PageList);
        }

        [HttpGet("load-all-feedback")]
        [Authorize(Roles = "Tutor")]
        public async Task<ActionResult<List<FeedbacksDTO>>> LoadAllFeedback(int pageNumber, int pageSize)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("cannot found user");

            var feedbacks = await _feedbackService.GetAllFeedBacks(username);
            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound(ApiResponse<string>.ErrorResult("You dont have any feedback"));
            }
            var PageList = feedbacks.ToPagedList(pageNumber, pageSize);

            return Ok(PageList);
        }

        [HttpPatch("hide-feedback")]
        [Authorize(Roles = "Tutor")]
        public async Task<ActionResult<string>> HideFeedback([FromQuery] int feedbackId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("cannot found user");

            if (feedbackId <= 0)
                return BadRequest("Feedback ID is not accept");

            var result = await _feedbackService.HideFeedback(username, feedbackId);
            if (result.Contains("Error"))
                return BadRequest(ApiResponse<string>.ErrorResult(result));

            return Ok(ApiResponse<string>.SuccessResult(result));
        }
        [Authorize]
        [HttpGet("check-user-can-give-feedback")]
        public async Task<ActionResult<bool>> CheckUserUsedTolearnTutor([FromQuery] int bookingId)
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(username))
                    return BadRequest("cannot found user");

                var checkBooking = await _feedbackService.CheckUserUsedToFeedbackOnBooking(bookingId);
                if (checkBooking)
                    return Ok(ApiResponse<bool>.SuccessResult(false));
                return Ok(ApiResponse<bool>.SuccessResult(true));
            } catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while check user to give feedback: {ex}"));
            }
        }

        [HttpPost("create-feedback")]
        [Authorize]
        public async Task<ActionResult<string>> CreateFeeback([FromBody] CreateFeedback feedback)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("cannot found user");

            if (feedback == null)
                return BadRequest("Feedback can not be null");

            var result = await _feedbackService.CreateFeedback(feedback, username);
            if (result.Contains("Error"))
                return BadRequest(ApiResponse<string>.ErrorResult(result));

            return Ok(ApiResponse<string>.SuccessResult(result));
        }
    }
}
