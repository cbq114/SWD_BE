using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }

        public int LanguageId { get; set; }
        public string UserName { get; set; }
        [Required]
        public string Address { get; set; }

        public decimal? Price { get; set; }
        [Required]
        public string Country { get; set; }
        public string TeachingExperience { get; set; }
        public string Education { get; set; }
        public TutorStatus? TutorStatus { get; set; }

        [ForeignKey("LanguageId")]
        public Languagues Subject { get; set; }
        [ForeignKey("UserName")]
        public Users User { get; set; }

        public virtual ICollection<Certifications> Certifications { get; set; }
    }
}
