using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> GetAllPromotion()
        {
            try
            {
                var tutor = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(tutor))
                    return Unauthorized(ApiResponse<string>.ErrorResult("User is not authenticated"));

                var resut = await _promotionService.GetAllPromotionOfTutor(tutor);
                if (!resut.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("You dont have any pronmotion"));

                return Ok(ApiResponse<List<PromotionDTO>>.SuccessResult(resut));
            } catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get all promotion of tutor: {ex}"));
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> CreatePromotion([FromForm] CreatePromotion createPromotion)
        {
            try
            {
                var tutor = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(tutor))
                    return Unauthorized(ApiResponse<string>.ErrorResult("User is not authenticated"));

                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.ErrorResult($"Invalid input data: {ModelState}"));

                // Check if dtb have other promotion have same tutor and code
                var isDuplicate = await _promotionService.CheckDuplicateCodeOfTutor(tutor, createPromotion.Code);
                if (isDuplicate)
                    return BadRequest(ApiResponse<string>.ErrorResult("Promotion with the same Code already exists."));

                createPromotion.Instructor = tutor;
                createPromotion.Status = PromotionStatus.Active;
                var result = await _promotionService.CreatePromotion(createPromotion);

                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while create promotion: {ex}"));
            }
        }

        [HttpGet("get-by-id/{promotionId}")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> GetById([FromRoute] int promotionId)
        {
            try
            {
                if (promotionId <= 0)
                    return BadRequest(ApiResponse<string>.ErrorResult("Promotion id required"));

                var result = await _promotionService.GetById(promotionId);

                if (result == null)
                    return BadRequest(ApiResponse<string>.ErrorResult($"Cannot find promotion with id: {promotionId}"));

                return Ok(ApiResponse<PromotionDTO>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get promotion by id: {ex}"));
            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> UpdatePromotion([FromForm] PromotionDTO newPromotion)
        {
            try
            {
                var tutor = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(tutor))
                    return Unauthorized(ApiResponse<string>.ErrorResult("User is not authenticated"));

                if (tutor != newPromotion.Instructor)
                    return Unauthorized(ApiResponse<string>.ErrorResult("You dont have permission to update this promotion"));

                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.ErrorResult($"Invalid input data: {ModelState}"));

                var result = await _promotionService.UpdatePromotion(newPromotion);

                return Ok(ApiResponse<PromotionDTO>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while update promotion: {ex}"));
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> DeletePromotion([FromRoute] int id)
        {
            try
            {
                var tutor = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(tutor))
                    return Unauthorized(ApiResponse<string>.ErrorResult("User is not authenticated"));

                var result = await _promotionService.DeletePromotion(tutor, id);
                if (result.Contains("Error"))
                    return BadRequest(ApiResponse<string>.ErrorResult(result));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while update promotion: {ex}"));
            }
        }

        [HttpGet("get-by-code")]
        [Authorize]
        public async Task<IActionResult> GetByCode([FromQuery] string code, [FromQuery] int bookingId)
        {
            try
            {
                var result = await _promotionService.GetByCode(code, bookingId);

                if (result == null)
                    return BadRequest(ApiResponse<string>.ErrorResult($"Cannot find promotion with code: {code}"));

                return Ok(ApiResponse<PromotionDTO>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get promotion by code: {ex}"));
            }
        }

        
    }
}
