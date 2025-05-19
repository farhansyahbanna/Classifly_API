namespace Classifly_API.DTOs.Requests
{
    public class UserRegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}
