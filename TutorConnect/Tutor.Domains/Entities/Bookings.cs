using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Bookings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }
        public string customer { get; set; }
        public int LessonId { get; set; }
        public int AvailabilityId { get; set; }
        public double? Price { get; set; }
        public BookingStatus Status { get; set; }
        public string Note { get; set; }
        public DateTime? Created { get; set; }

        [ForeignKey("customer")]
        public Users User { get; set; }
        [ForeignKey("LessonId")]
        public Lessons Lesson { get; set; }
        [ForeignKey("AvailabilityId")]
        public TutorAvailabilities TutorAvailability { get; set; }
        public virtual ICollection<PromotionUsage> PromotionUsages { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
        public virtual ICollection<Refunds> Refunds { get; set; }
        public virtual ICollection<Feedbacks> Feedbacks { get; set; }
        public virtual ICollection<LessonAttendances> LessonAttendances { get; set; }
    }

}
