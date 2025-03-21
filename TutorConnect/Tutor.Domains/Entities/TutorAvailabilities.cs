using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class TutorAvailabilities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TutorAvailabilityId { get; set; }
        public string Instructor { get; set; }
        public string meetingLink { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TutorAvailabilitityStatus Status { get; set; }


        [ForeignKey("Instructor")]
        public Users User { get; set; }
        public virtual ICollection<Bookings> Bookings { get; set; }
    }

}
