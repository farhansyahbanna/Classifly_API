using Classifly_API.DTOs.Requests;
using Classifly_API.Models;
using Classifly_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifly_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                FullName = request.FullName
            };

            try
            {
                var createdUser = await _authService.Register(user, request.Password);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var response = await _authService.Login(request.Username, request.Password);

            if (response == null)
                return Unauthorized("Username atau Password Salah");

            return Ok(response);
        }
    }
}
