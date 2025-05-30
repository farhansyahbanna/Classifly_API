namespace Classifly_API.DTOs.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
