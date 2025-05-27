using System.ComponentModel.DataAnnotations;

namespace Classifly_API.DTOs.Requests
{
    public class CategoryCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

    }
}
