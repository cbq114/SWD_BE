using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tutor,Manager")]
    public class PayoutController : ControllerBase
    {
        private readonly IDashboardService _service;

        public PayoutController(IDashboardService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPayout()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var payouts = await _service.GetPayoutsByUserNameAsync(username);
            return Ok(payouts);
        }
    }
}
