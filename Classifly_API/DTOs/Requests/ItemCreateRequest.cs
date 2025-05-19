namespace Classifly_API.DTOs.Requests
{
    public class ItemCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
