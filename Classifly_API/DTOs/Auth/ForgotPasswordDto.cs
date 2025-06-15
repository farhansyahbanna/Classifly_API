using System.ComponentModel.DataAnnotations;

namespace Classifly_API.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
