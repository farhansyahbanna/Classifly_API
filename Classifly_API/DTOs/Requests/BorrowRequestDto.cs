namespace Classifly_API.DTOs.Requests
{
    public class BorrowRequestDto
    {
        public int Id { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Flatten User data (avoid navigation property)
        public int UserId { get; set; }
        public string UserName { get; set; } // Example of flattened data

        // Include BorrowItems
        public List<BorrowItemDto> BorrowItems { get; set; }
    }

    public class BorrowItemDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } // Flattened
        public int Quantity { get; set; }
    }
}
