using Microsoft.AspNetCore.Mvc;
using Tutor.Applications.Interfaces;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatePriceController : ControllerBase
    {
        private readonly IRatePriceService _ratePriceService;

        public RatePriceController(IRatePriceService ratePriceService)
        {
            _ratePriceService = ratePriceService;
        }

        [HttpGet("current-rate")]
        public IActionResult GetCurrentRate()
        {
            var currentRate = _ratePriceService.GetCurrentRatePrice();
            return Ok(new { Rate = currentRate });
        }

        [HttpPut("update-rate")]
        public async Task<IActionResult> UpdateRate([FromBody] double rate)
        {
            if (rate <= 0)
            {
                return BadRequest(new { Message = "New rate must be greater than zero." });
            }

            var result = await _ratePriceService.UpdateRatePriceAsync(rate);

            if (result)
            {
                return Ok(new { Message = "Rate price updated successfully" });
            }
            else
            {
                return StatusCode(500, new { Message = "An error occurred while updating the rate price" });
            }
        }
    }
}
