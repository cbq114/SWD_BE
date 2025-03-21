using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Tutor.Domains.Entities
{
    public class FavoriteInstructors
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FavoriteInstructorId { get; set; }
        public string? UserName { get; set; }
        [ForeignKey("UserName")]
        public Users? User { get; set; }
        [JsonIgnore]
        public virtual ICollection<FavoriteInstructorDetails>? FavoriteInstructorDetails { get; set; }
    }
}
