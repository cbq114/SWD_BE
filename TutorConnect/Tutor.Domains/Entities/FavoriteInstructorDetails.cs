using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class FavoriteInstructorDetails
    {
        public int FavoriteInstructorId { get; set; }
        public string tutor { get; set; }

        public FavoriteStatus Status { get; set; }

        [ForeignKey("tutor")]
        public Users? user { get; set; }

        [ForeignKey("FavoriteInstructorId")]
        [JsonIgnore]
        public FavoriteInstructors? FavoriteInstructor { get; set; }
    }
}
