using Microsoft.AspNetCore.Mvc;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.HomeInstructorModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet()]
        public async Task<ActionResult<ApiResponse<List<InstructorDetailsModelHome>>>> SearchInstructors(
            [FromQuery] InstructorSearchOptions searchOptions)
        {
            try
            {
                var instructors = await _homeService.GetInstructorsForHomepageAsync(searchOptions);
                return Ok(ApiResponse<List<InstructorDetailsModelHome>>.SuccessResult(
                    instructors,
                    "Instructors retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                // Log exception here
                return BadRequest(ApiResponse<List<InstructorDetailsModelHome>>.ErrorResult(
                    "Error retrieving instructors: " + ex.Message
                ));
            }
        }

        [HttpGet("{instructorId}")]
        public async Task<IActionResult> GetInstructorDetails(string instructorId)
        {
            try
            {
                var instructorDetails = await _homeService.GetInstructorDetailsByIdAsync(instructorId);
                return Ok(ApiResponse<object>.SuccessResult(instructorDetails));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<object>.ErrorResult($"Error: {ex.Message}"));
            }
        }
    }
}
