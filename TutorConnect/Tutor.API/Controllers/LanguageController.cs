using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.Responses;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet("get_language")]
        public async Task<IActionResult> GetLanguageById(int id)
        {
            var lang = await _languageService.GetLanguageById(id);
            if (lang == null)
                return BadRequest(ApiResponse<string>.ErrorResult("Language not found"));

            return Ok(ApiResponse<LanguagesDTO>.SuccessResult(lang));
        }

        [HttpGet("get_all_language")]
        public async Task<IActionResult> GetAllLanguage()
        {
            var listLang = await _languageService.GetAllLanguages();
            if (listLang == null)
                return NotFound(ApiResponse<string>.ErrorResult("Language not found"));

            return Ok(ApiResponse<List<LanguagesDTO>>.SuccessResult(listLang));
        }

        [HttpPost("create_language")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateLanguage([FromForm] LanguagesDTO lang)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                if (username == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResult("User not found"));
                }
                lang.UserName = username;
                var result = await _languageService.CreateLanguage(lang);
                if (result == "Create language failed!" || result == "language cannot be null")
                    return BadRequest(ApiResponse<string>.ErrorResult("Invalid request data"));

                return Ok(ApiResponse<string>.SuccessResult(result));
            } catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while creating language: " + ex.Message));
            }
        }

        [HttpPut("update_language")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateLanguage([FromForm] LanguagesDTO lang)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                if (username == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResult("User not found"));
                }
                lang.UserName = username;
                var result = await _languageService.UpdateLanguage(lang);
                if (result == "Update failed!" || result == "Language not found!")
                    return BadRequest(ApiResponse<string>.ErrorResult("Invalid request data"));

                return Ok(ApiResponse<string>.SuccessResult(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResult($"An error occurred while updating language: " + ex.Message));
            }
        }

        [HttpDelete("delete_language")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            try
            {
                if (id == 0)
                    return BadRequest(ApiResponse<string>.ErrorResult("LanguageId is required for delete"));

                var result = await _languageService.DeleteLanguage(id);
                if (result == "Can not find languages with this id")
                    return BadRequest(ApiResponse<string>.ErrorResult(result));
                return Ok(ApiResponse<string>.SuccessResult(result));

            }
            catch (Exception ex)
            {

                return StatusCode(500, ApiResponse<string>.ErrorResult("An error occurred while deleting language: " + ex.Message));
            }
        }
    }
}
