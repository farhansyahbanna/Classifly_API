namespace Classifly_API.DTOs.Requests
{
    public class ItemUpdateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile ImageFile { get; set; }
        public string ExistingImageUrl { get; set; }
    }
}
