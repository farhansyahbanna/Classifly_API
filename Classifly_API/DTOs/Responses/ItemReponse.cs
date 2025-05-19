namespace Classifly_API.DTOs.Responses
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
