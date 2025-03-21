using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Feedbacks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        public int? Star { get; set; }
        public string? Comment { get; set; }
        public string username { get; set; }
        public int? BookingId { get; set; }

        public FeedbackStatus Status { get; set; }
        [ForeignKey("username")]
        public Users? User { get; set; }
        [ForeignKey("BookingId")]
        public Bookings? Booking { get; set; }

    }
}
