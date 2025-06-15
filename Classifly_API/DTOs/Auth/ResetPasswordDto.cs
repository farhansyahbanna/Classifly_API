using System.ComponentModel.DataAnnotations;

namespace Classifly_API.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password minimal 8 karakter")]
        public string NewPassword
        {
            get; set;
        }
    }
}
