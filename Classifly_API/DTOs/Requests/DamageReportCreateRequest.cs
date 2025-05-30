namespace Classifly_API.DTOs.Requests
{
    public class DamageReportCreateRequest
    {
        public int BorrowRequestId { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

    }
}
