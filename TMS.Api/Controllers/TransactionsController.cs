using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Enums;

namespace TMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private bool GetUserId(out Guid userId)
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out userId);
        }

        [HttpPost]
        [Authorize(Roles = "Teller")]
        public async Task<IActionResult> CreateTransactionAsync(CreateTransactionDto dto)
        {
            var tellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(tellerId, out Guid tellerGuid))
            {
                return Unauthorized(new { message = "Invalid teller ID in token." });
            }

            var transaction = await _transactionService.CreateTransactionAsync(dto, tellerGuid);

            return Ok(transaction);
        }

        [HttpGet("GetMyTransactions")]
        [Authorize(Roles = "Teller")]
        public async Task<ActionResult<List<TransactionDto>>> GetMyTransactions()
        {
            if (!GetUserId(out Guid tellerId))
                return Unauthorized(new { message = "Invalid teller ID in token." });

            var result = await _transactionService.GetTransactionByTellerIdAsync(tellerId);
            return Ok(result);
        }

        [HttpGet("GetAllTransactions")]
        [Authorize(Roles = "HOP, IC")]
        public async Task<ActionResult<List<TransactionDto>>> GetAllTransactions([FromQuery] TransactionType? transactionType, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
        {
            var result = await _transactionService.GetAllTransactionsAsync(transactionType, fromUtc, toUtc);
            return Ok(result);
        }

        [HttpGet("GetTransactionById")]
        [Authorize(Roles = "HOP, IC")]
        public async Task<ActionResult<TransactionDto?>> GetTransactionById(Guid transactionId)
        {
            if (GetUserId(out var userId))
                return Unauthorized(new { message = "Invalid user ID in token." });

            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            var result = await _transactionService.GetTransactionByIdAsync(transactionId, userId, role);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
