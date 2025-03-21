using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.Responses;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _service;

        public CertificationController(ICertificationService service)
        {
            _service = service;
        }

        private string GetUsername()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        private IActionResult ValidateUserAndId(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid certification ID.");

            var userName = GetUsername();
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User not authenticated.");

            return null;
        }
        [HttpGet("view-certificate-by/{username}")]
        public async Task<IActionResult> GetCertificate(string username)
        {
            var certification = await _service.GetCerByUsernameAsync1(username);
            if (certification == null)
                return NotFound(ApiResponse<string>.ErrorResult($"Certification not found for user {username}."));

            return Ok(certification);

        }

        [HttpGet("view-all-certificate")]
        [Authorize(Roles = "Tutor")]

        public async Task<IActionResult> Get()
        {
            var userName = GetUsername();
            var certification = await _service.GetCerByUsernameAsyncs(userName);
            if (!certification.Any())
                return NotFound(ApiResponse<string>.ErrorResult($"Certification not found for user {userName}."));

            return Ok(ApiResponse<List<CertificationsDTO>>.SuccessResult(certification));
        }

        [HttpPut("update-id/{id}")]
        [Authorize(Roles = "Tutor")]

        public async Task<IActionResult> Put(int id, [FromForm] UpdateCertification model)
        {
            var validationResult = ValidateUserAndId(id);
            if (validationResult != null) return validationResult;

            if (model == null)
                return BadRequest("Invalid model data.");

            try
            {
                var userName = GetUsername();
                var updatedCertification = await _service.UpdateAsync(id, model, userName);
                if (updatedCertification == null)
                    return NotFound($"Certification with ID {id} not found or not accessible for user {userName}.");

                return Ok(new { Message = "Update successful", Certification = updatedCertification });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Tutor")]

        public async Task<IActionResult> Delete(int id)
        {
            var validationResult = ValidateUserAndId(id);
            if (validationResult != null) return validationResult;
            var userName = GetUsername();
            if (userName == null)
            {
                return BadRequest("You can login.");
            }
            await _service.DeleteAsync(id);
            return Ok(new { Message = "Delete successful" });

        }
        [HttpPost("add-certification")]
        [Authorize(Roles = "Tutor")]

        public async Task<IActionResult> Add([FromForm] AddCertification model)
        {
            if (model == null || model.Certification == null)
                return BadRequest(ApiResponse<string>.ErrorResult("Certification model or file cannot be null."));

            var userName = GetUsername();
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(ApiResponse<String>.ErrorResult("User not authenticated."));

            try
            {
                var result = await _service.AddAsync(model, userName);
                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
