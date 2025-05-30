namespace Classifly_API.Models
{
    
    public class BorrowRequest
    {
        public int Id { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminMessage { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<BorrowItem> BorrowItems { get; set; } = new List<BorrowItem>();

        public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();

    }

}
