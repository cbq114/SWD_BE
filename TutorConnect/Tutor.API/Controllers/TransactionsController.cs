using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Applications.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Models.Responses;

namespace Tutor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("get_all")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var trans = await _transactionService.GetAllTransactions();
            if(trans == null)
                return NotFound(ApiResponse<string>.ErrorResult("Can not found any transaction"));

            return Ok(ApiResponse<List<TransactionDTO>>.SuccessResult(trans));
        }
        [Authorize(Roles = "Tutor")]
        [HttpGet("get_all_of")]
        public async Task<IActionResult> GetAllTransactionOfUser()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (username == null)
                return NotFound(ApiResponse<string>.ErrorResult("User not found"));

            var trans = await _transactionService.GetAllTransactionsOfUser(username);
            if (trans == null)
                return NotFound(ApiResponse<string>.ErrorResult("Can not found any transaction"));

            return Ok(ApiResponse<List<TransactionDTO>>.SuccessResult(trans));
        }
    }
}
