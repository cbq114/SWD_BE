using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaypalController : ControllerBase
    {
        private readonly IPaypalService paypalService;

        public PaypalController(IPaypalService paypalService)
        {
            this.paypalService = paypalService;
        }

        [HttpPost("request-payout")]
        [Authorize(Roles = "Tutor,Student,Manager")]
        public async Task<IActionResult> RequestPayout([FromBody] PayoutRequest request)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email Invalid!");
            }

            var result = await paypalService.RequestPayoutAsync(email, request.Amount, request.Reason);
            if (!result.Success)
            {
                return StatusCode(500, result.Message);
            }

            return Ok(new { success = result.Success, message = result.Message });
        }

        [HttpPost("approve-payout")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePayout([FromQuery] bool isApproved, [FromBody] ApprovePayoutRequest request)
        {
            if (request.PayoutId <= 0)
            {
                return BadRequest("Valid Payout ID is required.");
            }

            var payoutResult = await paypalService.ProcessApprovedPayoutAsync(request.PayoutId, isApproved, request.Comment);
            if (!payoutResult.Success)
            {
                return StatusCode(500, payoutResult.Message);
            }

            return Ok(new { success = payoutResult.Success, message = payoutResult.Message });
        }
        [HttpGet("balance-Admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBalance()
        {
            var balance = await paypalService.GetBalanceAsync();

            if (balance.HasValue)
            {
                return Ok(new { Balance = balance.Value, Currency = "USD" });
            }

            return StatusCode(500, "Failed to retrieve balance.");
        }
        public class PayoutRequest
        {
            public double Amount { get; set; }
            public string Reason { get; set; }
        }
        public class ApprovePayoutRequest
        {
            public int PayoutId { get; set; }
            public string Comment { get; set; }
        }
    }
}
