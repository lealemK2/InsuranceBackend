﻿namespace Insurance.Dto
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool CorrectPassword { get; set; } = true;
    }
}
