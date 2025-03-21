using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Tutor.Infratructures.Models.Authen
{
    public class UpgradeToInstructorModel
    {
        [Required]
        public string Address { get; set; }
        [Required]
        public string Country { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public decimal? Price { get; set; }
        [Required]
        [MinLength(50, ErrorMessage = "Teaching experience must be at least 50 characters long.")]
        public string TeachingExperience { get; set; }

        [Required]
        [MinLength(50, ErrorMessage = "Education information must be at least 50 characters long.")]
        public string Education { get; set; }

        [Required]
        public IFormFile Document { get; set; }
    }
}
