using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;


namespace TMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize(Roles = "TELLER")]
        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
        {
            var tellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(tellerId, out Guid tellerGuid))
            {
                return Unauthorized(new { message = "Invalid teller ID in token." });
            }

            var transaction = await _transactionService.CreateTransaction(dto, tellerGuid);

            return Ok(transaction);
        }

        [Authorize(Roles = "TELLER")]
        [HttpGet("my-transactions")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var tellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(tellerId, out Guid tellerGuid))
            {
                return Unauthorized(new { message = "Invalid teller ID in token." });
            }

            var transactions = await _transactionService.GetTransactionByTellerId(tellerGuid);

            return Ok(transactions);
        }

        [HttpGet]
        [Authorize(Roles ="HOP, IC")]
        public async Task<IActionResult> GetAllTransactions([FromQuery] Domain.Enums.TransactionType? type, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
        {
            if (type.HasValue && !Enum.IsDefined(typeof(Domain.Enums.TransactionType), type.Value))
            {
                return BadRequest("Invalid Transaction type");
            }

            if (fromUtc.HasValue && toUtc.HasValue && fromUtc.Value >= toUtc.Value)
            {
                return BadRequest("fromutc must be earlier than to utc");
            }

            var transactions = await _transactionService.GetAllTransactions(type, fromUtc, toUtc);

            return Ok(transactions);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "TELLER, HOP, IC")]
        public async Task<IActionResult> GetTransactionById(Guid Id)
        {
            var tellerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(tellerId, out Guid tellerGuid))
            {
                return Unauthorized(new { message = "Invalid teller ID in token." });
            }

            var transactions = await _transactionService.GetTransactionById(Id);

            if (transactions == null)
            {
                return NotFound("Transaction not found");
            }

            return Ok(transactions);
        }
    }
}
