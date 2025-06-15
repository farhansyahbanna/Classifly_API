using Classifly_API.DTOs.Auth;
using Classifly_API.DTOs.Requests;
using Classifly_API.DTOs.Responses;
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

        

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var response = await _authService.Login(request.Username, request.Password);

            if (response == null)
                return Unauthorized("Username atau Password Salah");

            return Ok(response);
        }


        [HttpPost("request-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestOtp([FromBody] ForgotPasswordRequest request)
        {
            await _authService.RequestPasswordResetOtpAsync(request.Email);
            return Ok(new { message = "Jika email Anda terdaftar, kode OTP telah dikirim." });
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var finalResetToken = await _authService.VerifyOtpAndGenerateResetTokenAsync(request);
            if (finalResetToken == null)
            {
                return BadRequest(new { message = "Kode OTP salah atau telah kedaluwarsa." });
            }
            // Kirim "tiket" reset ke frontend
            return Ok(new { resetToken = finalResetToken });
        }

        [HttpPost("reset-password")] 
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithToken([FromBody] ResetPasswordRequest request)
        {
            var success = await _authService.ResetPasswordWithTokenAsync(request);
            if (!success)
            {
                return BadRequest(new { message = "Token reset tidak valid atau telah kedaluwarsa." });
            }
            return Ok(new { message = "Password Anda telah berhasil diubah." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/count")]
        public async Task<IActionResult> GetUsersCount()
        {
            try
            {
                var count = await _authService.GetTotalUsersCount();
                return Ok(new { totalUsers = count });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Terjadi kesalahan internal pada server.");
            }
        }

        [HttpPost("admin/create-user")]
        [Authorize(Roles = "Admin")] 
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.CreateUserByAdminAsync(userDto);

            if (!result.Succeeded)
            {
   
                return BadRequest(new { message = result.ErrorMessage });
            }

      
            return CreatedAtAction(nameof(GetUserById), new { id = result.User.Id }, result.User);
        }

        [HttpGet("{id}")]
        [Authorize] 
        public async Task<IActionResult> GetUserById(int id)
        {

            var user = await _authService.GetUserById(id); 
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}
