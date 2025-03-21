using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tutor,Manager")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _wallet;

        public WalletController(IWalletRepository wallet)
        {
            _wallet = wallet;
        }
        [HttpGet("get-my-wallet")]
        public async Task<IActionResult> GetWallet()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User is not authenticated");
            }

            try
            {
                var wallet = await _wallet.GetWalletByUsername(username);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

    }
}
