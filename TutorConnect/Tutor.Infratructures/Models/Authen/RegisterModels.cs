using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Tutor.Shared.Exceptions;

namespace Tutor.Infratructures.Models.Authen
{
    public class RegisterBaseModel
    {
        [RegularExpression(@"^[a-zA-Z0-9\sÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẮẰẲẴẶắằẳẵặƯứừửữự]+$", ErrorMessage = "UserName must not contain special characters.")]
        [MinLength(5, ErrorMessage = "User name must be at least 5 characters long.")]
        public string UserName { get; set; }

        //public int RoleId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [RegularExpression(@"^[\w-\.]+@(gmail\.com|fpt\.edu\.vn)$", ErrorMessage = "Email must be a valid Gmail or FPT email address.")]
        public string Email { get; set; }

        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must  be begin 0 and 10 digits long.")]
        public string PhoneNumber { get; set; }

        [MinLength(10, ErrorMessage = "Full name must be at least 10 characters long.")]
        [MaxLength(200, ErrorMessage = "Full name max 200 characters long.")]
        [RegularExpression(@"^[^!@#$%^&*()_+=\[{\]};:<>|./?,-]*$", ErrorMessage = "Full name must not contain special characters.")]
        public string? FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [ValidateDOB(ErrorMessage = "You must be at least 16 years old.")]
        public DateTime DOB { get; set; }

    }
    public class RegisterInstructorModel : RegisterBaseModel
    {
        [Required(ErrorMessage = "Address is required")]
        [MinLength(10, ErrorMessage = "Address must be at least 10 characters long.")]
        [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Price per lesson is required")]
        public decimal? Price { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public int LanguageId { get; set; }
        [Required]
        public IFormFile Avatar { get; set; }

        [Required]
        public IFormFile CVFile { get; set; }

        [Required(ErrorMessage = "Teaching experience is required")]
        [MinLength(50, ErrorMessage = "Teaching experience must be at least 50 characters long.")]
        [MaxLength(2000, ErrorMessage = "Teaching experience cannot exceed 2000 characters.")]
        public string TeachingExperience { get; set; }

        [Required(ErrorMessage = "Education information is required")]
        [MinLength(50, ErrorMessage = "Education information must be at least 50 characters long.")]
        [MaxLength(2000, ErrorMessage = "Education information cannot exceed 2000 characters.")]
        public string Education { get; set; }
    }
}
