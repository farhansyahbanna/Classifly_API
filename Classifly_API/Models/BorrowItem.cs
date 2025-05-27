namespace Classifly_API.Models
{
    public class BorrowItem
    {
        public int Id { get; set; }
        public int BorrowRequestId { get; set; }
        public BorrowRequest BorrowRequest { get; set; } = null!;

        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
