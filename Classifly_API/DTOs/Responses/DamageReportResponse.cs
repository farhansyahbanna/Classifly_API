namespace Classifly_API.DTOs.Responses
{
    public class DamageReportResponse
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminMessage { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int BorrowRequestId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
