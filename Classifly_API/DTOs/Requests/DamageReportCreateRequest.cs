namespace Classifly_API.DTOs.Requests
{
    public class DamageReportCreateRequest
    {
        public int ItemId { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Location { get; set; }
    }
}
