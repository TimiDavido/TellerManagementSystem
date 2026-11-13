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
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly IChangePasswordService _changePasswordService;
        private readonly IRefreshTokenApplicationService _refreshTokenApplicationService;
        private readonly ILogoutService _logoutService;

        public UsersController(IUserService userService, ILoginService loginService, IChangePasswordService changePasswordService, IUserRepository userRepository, IRefreshTokenApplicationService refreshTokenApplicationService, ILogoutService logoutService)
        {
            _userService = userService;
            _loginService = loginService;
            _changePasswordService = changePasswordService;
            _userRepository = userRepository;
            _refreshTokenApplicationService = refreshTokenApplicationService;
            _logoutService = logoutService;
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
                var createdById = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(createdById, out Guid createdBy))
                {
                    return Unauthorized(new { message = "Invalid user ID in token." });
                }

                var user = await _userService.CreateUser(createUserDto, createdBy);
                return StatusCode(StatusCodes.Status201Created, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
                var token = await _loginService.Login(loginDto.Email, loginDto.Password);

                return Ok(new
                {
                    accessToken = token.AccessToken,
                    refreshToken = token.RefreshToken,
                    tokenType = "Bearer"
                });
        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
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

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
                var tokens = await _refreshTokenApplicationService.RefreshToken(request.RefreshToken);

                return Ok(new
                {
                    accessToken = tokens.AccessToken,
                    refreshToken = tokens.RefreshToken,
                    tokenType = "Bearer"
                });
        }

        [HttpPost("logout")]
        [AllowAnonymous]               
        public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
        {
            await _logoutService.Logout(request.RefreshToken);

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }
    }
}
