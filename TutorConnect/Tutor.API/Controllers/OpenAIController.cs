using Microsoft.AspNetCore.Mvc;
using Tutor.Applications.Interfaces;
using Tutor.Applications.Services;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;

        public OpenAIController(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskOpenAI([FromBody] OpenAIRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Prompt))
                {
                    return BadRequest("Prompt cannot be empty");
                }

                var response = await _openAIService.GenerateResponse(request.Prompt);
                return Ok(new { message = response });
            }
            catch (OpenAIQuotaExceededException ex)
            {
                return StatusCode(429, new { error = "Quota exceeded", message = ex.ApiErrorMessage });
            }
            catch (OpenAIException ex)
            {
                return StatusCode(500, new { error = ex.Message, message = ex.ApiErrorMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }

    public class OpenAIRequest
    {
        public string? Prompt { get; set; }
    }
}