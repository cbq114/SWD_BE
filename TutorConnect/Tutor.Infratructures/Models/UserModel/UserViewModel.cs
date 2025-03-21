using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.UserModel
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public DateTime DOB { get; set; }
    }
    public class UserViewInstructorModel : UserViewModel
    {
        public string Address { get; set; }
        public string TeachingExperience { get; set; }
        public string Education { get; set; }
        public decimal? Price { get; set; }

        public int LanguageId { get; set; }
        public string Country { get; set; }
    }
    public class UserModelDTO
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public DateTime DOB { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserStatus Status { get; set; }
    }
}
