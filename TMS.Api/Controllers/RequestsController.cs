using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;
using TMS.Domain.Enums;

namespace TMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(value!);
        }

        [HttpGet("allRequests")]
        [Authorize(Roles = "HOP, IC")]
        public async Task<ActionResult<List<RequestDto>>> GetAllRequests([FromQuery] RequestStatus? status)
        {
            var result = await _requestService.GetAllRequestsAsync(status);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<RequestDto>> Create(CreateRequestDto createRequestDto)
        {
            var result = await _requestService.CreateRequestAsync(GetCurrentUserId(), createRequestDto);
            return Ok(result);
        }

        [HttpGet("myRequest")]
        public async Task<ActionResult<List<RequestDto>>> GetMyRequest()
        {
            var result = await _requestService.GetMyRequestsAsync(GetCurrentUserId());
            return Ok(result);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "HOP, IC")]
        public async Task<ActionResult<List<RequestDto>>> GetPendingRequests()
        {
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var result = await _requestService.GetPendingRequestsAsync(role);
            return Ok(result);
        }

        [HttpPut("{id:guid}/review")]
        [Authorize(Roles = "HOP, IC")]
        public async Task<ActionResult<RequestDto>> Review(Guid id, ApproveRequestDto approveRequestDto)
        {
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var result = await _requestService.ApproveRequestAsync(id, GetCurrentUserId(), role, approveRequestDto);
            return Ok(result);
        }
    }
}