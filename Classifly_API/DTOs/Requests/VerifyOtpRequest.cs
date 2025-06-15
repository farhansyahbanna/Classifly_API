using System.ComponentModel.DataAnnotations;

namespace Classifly_API.DTOs.Requests
{
    public class VerifyOtpRequest
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP harus 6 digit.")]
        public required string Otp { get; set; }
    }
}
