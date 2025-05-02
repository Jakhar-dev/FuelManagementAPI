using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModels;
using FuelManagementAPI.Repositories;
using FuelManagementAPI.Repositories.IRepositories;
using FuelManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FuelManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;
        private readonly ICategoryRepository _categoryRepository;

        public AuthController(IUserRepository userRepository, AuthService authService, ICategoryRepository categoryRepository)
        {
            _userRepository = userRepository;
            _authService = authService;
            _categoryRepository = categoryRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepository.MobileExistsAsync(model.Mobile))
                return BadRequest("Mobile number already exists.");

            var user = new User
            {
                Name = model.Name,
                Mobile = model.Mobile,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = "manager"
            };

            await _userRepository.CreateAsync(user);

            var defaultCategories = new List<ProductCategory>
            {
                new ProductCategory { CategoryName = "Fuel", Description = "Diesel, Petrol Etc.", UsersId = user.UsersId },
                new ProductCategory { CategoryName = "Lube", Description = "Engine oil etc.", UsersId = user.UsersId}
            };
            await _categoryRepository.AddCategoriesAsync(defaultCategories);
                return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetByMobileAsync(model.Mobile);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("Invalid mobile number or password.");

            var accessToken = _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { token = accessToken, userId = user.UsersId });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token missing.");

            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            var newAccessToken = _authService.GenerateJwtToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { token = newAccessToken });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userMobile = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { userId, userName, userMobile, userRole });
        }
    }
}
