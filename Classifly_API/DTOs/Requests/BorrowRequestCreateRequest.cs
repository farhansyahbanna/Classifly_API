namespace Classifly_API.DTOs.Requests
{
    public class BorrowRequestCreateRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Location { get; set; }
    }
}
