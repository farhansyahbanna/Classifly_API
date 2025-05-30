namespace Classifly_API.Models
{
    public class DamageReport
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BorrowRequestId { get; set; }
        public BorrowRequest BorrowRequest { get; set; } = null!;
  

    }
}
