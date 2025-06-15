using System.ComponentModel.DataAnnotations;

namespace Classifly_API.DTOs.Requests
{
    public class CategoryUpdateRequest
    {
        [Required(ErrorMessage = "Nama kategori tidak boleh kosong.")]
        [StringLength(100, ErrorMessage = "Nama kategori maksimal 100 karakter.")]
        public required string CategoryName { get; set; }

    }
}
