using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TMS.Application.DTOs;
using TMS.Application.Interfaces;

namespace TMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginService _loginService;
        private readonly IChangePasswordService _changePasswordService;

        public UsersController(IUserRepository userRepository, ILoginService loginService, IChangePasswordService changePasswordService)
        {
            _userRepository = userRepository;
            _loginService = loginService;
            _changePasswordService = changePasswordService;
        }

        [Authorize(Roles = "HOP, IC")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles ="HOP")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {     
            try
            {
                var createdById = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(createdById, out Guid createdBy))
                {
                    return Unauthorized(new { message = "Invalid user ID in token." });
                }

                var user = await _userRepository.CreateUser(createUserDto, createdBy);
                return StatusCode(StatusCodes.Status201Created, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var token = await _loginService.Login(loginDto.Email, loginDto.Password);

                if (token == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                return Ok(new
                {
                    accessToken = token,
                    tokenType = "Bearer"
                });
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdValue, out Guid userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token." });
                }

                var success = await _changePasswordService.ChangePassword(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!success)
                {
                    return BadRequest(new { message = "Password change failed. Please check your current password and try again." });
                }

                return Ok(new { message = "Password changed successfully." });
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            };
            
        }
    }
}
