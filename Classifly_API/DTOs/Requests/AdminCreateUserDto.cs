using System.ComponentModel.DataAnnotations;

namespace Classifly_API.DTOs.Requests
{
    public class AdminCreateUserDto
    {
        [Required(ErrorMessage = "Username harus diisi.")]
        [MinLength(3, ErrorMessage = "Username minimal 3 karakter.")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Email harus diisi.")]
        [EmailAddress(ErrorMessage = "Format email tidak valid.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password harus diisi.")]
        [MinLength(8, ErrorMessage = "Password minimal 8 karakter.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Nama lengkap harus diisi.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Role harus diisi.")]
        public required string Role { get; set; }
    }
}
