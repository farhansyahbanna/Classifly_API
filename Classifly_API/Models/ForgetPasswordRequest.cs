using System.ComponentModel.DataAnnotations;

namespace Classifly_API.Models
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email harus diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        public required string Email { get; set; }
    }
}
