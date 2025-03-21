﻿using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.UserModel
{
    public class SearchOptionUser
    {
        public string? FullName { get; set; }
        public string? LanguageName { get; set; }
        public string? Country { get; set; }
        public TutorStatus? TutorStatus { get; set; }
    }
    public class UserFilter
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public UserStatus? Status { get; set; }
    }

}
