namespace Classifly_API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<BorrowRequest> BorrowRequests { get; set; }
        public ICollection<DamageReport> DamageReports { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
