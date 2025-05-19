namespace Classifly_API.Models
{
    
    public class BorrowRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Location { get; set; }
        public string Status { get; set; } = "Pending";
        public string AdminMessage { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Item Item { get; set; }
    }
    
}
