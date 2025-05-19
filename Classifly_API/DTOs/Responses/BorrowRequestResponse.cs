namespace Classifly_API.DTOs.Responses
{
    public class BorrowRequestResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string AdminMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
