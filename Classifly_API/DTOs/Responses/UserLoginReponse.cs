﻿namespace Classifly_API.DTOs.Responses
{
    public class UserLoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Token { get; set; }
    }
}
