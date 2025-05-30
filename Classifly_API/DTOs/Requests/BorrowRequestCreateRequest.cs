namespace Classifly_API.DTOs.Requests
{
    public class BorrowRequestCreateRequest
    {
        public int Id { get; set; }
        public DateTime BorrowDate { get; set; }
        // public DateTime ReturnDate { get; set; }
        
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<BorrowItemDto> Items { get; set; } = new();
    }

   public class BorrowItemDto
    {
        public int ItemId { get; set; }
        //  public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
