﻿using static LangApp.Shared.Models.Enums;

namespace LangApp.Shared.Models
{
    public class RegisterCredentials
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole UserRole { get; set; }
    }
}
