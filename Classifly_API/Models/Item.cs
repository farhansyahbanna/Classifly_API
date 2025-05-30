namespace Classifly_API.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ICollection<BorrowItem> BorrowItems { get; set; }
        public Category Category { get; set; }
        public ICollection<BorrowRequest> BorrowRequests { get; set; }
        public ICollection<DamageReport> DamageReports { get; set; }
    }
}
