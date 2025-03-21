using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class LessonAttendances
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonAttendanceId { get; set; }
        public int BookingId { get; set; }
        public string note { get; set; }
        public bool? IsAttended { get; set; }
        public DateTime CreateAt { get; set; }

        [ForeignKey("BookingId")]
        public Bookings Booking { get; set; }

        public virtual ICollection<LessonAttendanceDetails> AttendanceDetails { get; set; }
    }
}
