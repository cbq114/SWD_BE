using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Models.Authen;
using Tutor.Infratructures.Models.Responses;
using Tutor.Infratructures.Models.UpgradeModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpgradeRequestController : ControllerBase
    {
        private readonly IUpgradeRequestService _service;
        private readonly ILogger<UpgradeRequestController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpgradeRequestController(IUpgradeRequestService service, ILogger<UpgradeRequestController> logger, IUnitOfWork unitOfWork)
        {
            _service = service;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Submit an upgrade request to become an instructor.("Customer")
        /// </summary>
        /// <remarks>
        /// This endpoint allows a student to request an upgrade to an instructor role.
        /// </remarks>
        /// <param name="model">Upgrade request details</param>
        /// <returns>A success or error message</returns>
        /// <response code="200">Request submitted successfully</response>
        /// <response code="400">Request already exists</response>
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        public async Task<ActionResult<ApiResponse<string>>> CreateUpgradeRequest([FromForm] UpgradeToInstructorModel model)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _service.CreateUpgradeRequest(username, model);

            if (result.Contains("already"))
                return BadRequest(ApiResponse<string>.ErrorResult(result));

            await _unitOfWork.SaveChangesAsync();
            return Ok(ApiResponse<string>.SuccessResult(result));
        }

        /// <summary>
        /// Get all pending upgrade requests.(Admin)
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves a list of all pending upgrade requests for admin review.
        /// </remarks>
        /// <returns>A list of pending upgrade requests</returns>
        /// <response code="200">Returns a list of pending requests</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<UpgradeRequestDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<ActionResult<ApiResponse<List<UpgradeRequestDto>>>> GetPendingRequests()
        {
            try
            {
                var requests = await _service.GetPendingRequests();
                return Ok(ApiResponse<List<UpgradeRequestDto>>.SuccessResult(requests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending requests");
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while retrieving requests"));
            }
        }

        /// <summary>
        /// Approve an upgrade request.(Admin)
        /// </summary>
        /// <remarks>
        /// This endpoint allows an admin to approve a student's request to become an instructor.
        /// </remarks>
        /// <param name="requestId">The ID of the upgrade request</param>
        /// <returns>Success or failure status</returns>
        /// <response code="200">Request approved successfully</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{requestId}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveRequest(int requestId)
        {
            try
            {
                var result = await _service.ApproveRequest(requestId);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResult("Request not found or already processed"));

                await _unitOfWork.SaveChangesAsync();
                return Ok(ApiResponse<bool>.SuccessResult(true, "Request approved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving request {RequestId}", requestId);
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while processing the request"));
            }
        }

        /// <summary>
        /// Reject an upgrade request.(Admin)
        /// </summary>
        /// <remarks>
        /// This endpoint allows an admin to reject a student's request to become an instructor.
        /// </remarks>
        /// <param name="requestId">The ID of the upgrade request</param>
        /// <returns>Success or failure status</returns>
        /// <response code="200">Request rejected successfully</response>
        /// <response code="404">Request not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{requestId}/reject")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> RejectRequest(int requestId, [FromBody] RejectRequestDto request)
        {
            try
            {
                var result = await _service.RejectRequest(requestId, request.Reason);
                if (!result)
                    return NotFound(ApiResponse<bool>.ErrorResult("Request not found or already processed"));

                await _unitOfWork.SaveChangesAsync();
                return Ok(ApiResponse<bool>.SuccessResult(true, "Request rejected successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting request {RequestId}", requestId);
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while processing the request"));
            }
        }
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<List<UpgradeRequestDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 500)]
        public async Task<ActionResult<ApiResponse<List<UpgradeRequestDto>>>> GetRequests()
        {
            try
            {
                var requests = await _service.GetAllRequests();
                return Ok(ApiResponse<List<UpgradeRequestDto>>.SuccessResult(requests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending requests");
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while retrieving requests"));
            }
        }
        public class RejectRequestDto
        {
            public string Reason { get; set; }
        }

    }
}
