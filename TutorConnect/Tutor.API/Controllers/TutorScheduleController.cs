using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.AvailiabilityModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorScheduleController : ControllerBase
    {
        private readonly ITutorScheduleService _tutorScheduleService;

        public TutorScheduleController(ITutorScheduleService tutorScheduleService)
        {
            _tutorScheduleService = tutorScheduleService ?? throw new ArgumentNullException(nameof(tutorScheduleService));
        }

        /// <summary>
        /// Add new tutor availability schedule
        /// </summary>
        [HttpPost("schedules")]
        [Authorize(Roles = "Tutor")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddSchedule([FromBody] CreateTutorAvailabilityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.ErrorResult("Invalid model state"));
            }

            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
            {
                return Unauthorized(ApiResponse<string>.ErrorResult("Instructor ID not found"));
            }

            try
            {
                var result = await _tutorScheduleService.AddTutorAvailability(model, instructorId);
                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }
        /// <summary>
        /// Update existing tutor schedule
        /// </summary>
        [HttpPut("schedules/{id}")]
        [Authorize(Roles = "Tutor")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSchedule([FromBody] CreateTutorAvailabilityModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("Invalid model state"));
            }

            try
            {
                var result = await _tutorScheduleService.UpdateTutorAvailability(model, id);
                if (result == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Schedule with ID {id} not found"));
                }
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Delete tutor schedule by ID
        /// </summary>
        [HttpDelete("schedules/{id}")]
        [Authorize(Roles = "Tutor")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var result = await _tutorScheduleService.DeleteTutorAvailability(instructorId, id);
                if (!result)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Schedule with ID {id} not found"));
                }
                return Ok(ApiResponse<object>.SuccessResult(true, "Schedule deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }


        /// <summary>
        /// Get all schedules for a specific instructor (Show for student) // dành cho student
        /// </summary>
        [HttpGet("schedules/instructor/{instructorId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInstructorSchedules(string instructorId)
        {
            try
            {
                var result = await _tutorScheduleService.GetAllScheduleforStudent(instructorId);
                if (result == null || !result.Any())
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"No schedules found for instructor {instructorId}"));
                }
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Get all schedules for a specific instructor // dành cho instructor//(ở dashboard)
        /// </summary>
        [HttpGet("schedules/instructor")]
        [Authorize(Roles = "Tutor")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInstructorSchedulesIns()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var result = await _tutorScheduleService.GetAllScheduleforTutor(username);
                if (result == null || !result.Any())
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"No schedules found for instructor {username}"));
                }
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// Get schedule by ID
        /// </summary>
        [HttpGet("schedules/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            try
            {
                var result = await _tutorScheduleService.getScheduleById(id);
                if (result == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult($"Schedule with ID {id} not found"));
                }
                return Ok(ApiResponse<object>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
        [HttpGet("changeStatus/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeStatus(int id, TutorAvailabilitityStatus status)
        {
            try
            {
                await _tutorScheduleService.ChangeTutorAvailStatus(id, status);

                return Ok("Change status successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
        }
    }
}