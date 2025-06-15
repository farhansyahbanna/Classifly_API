using System.ComponentModel.DataAnnotations;

namespace Classifly_API.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public required string Token { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password minimal harus 8 karakter.")]
        public required string NewPassword { get; set; }
    }
}
