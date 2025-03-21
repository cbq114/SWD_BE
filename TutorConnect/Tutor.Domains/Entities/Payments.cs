using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Payments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string? PaymentCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public double? Amount { get; set; }
        public string? Description { get; set; }
        public PaymentStatus Status { get; set; }
        public bool IsPaid { get; set; } = false;

        [ForeignKey("BookingId")]
        public Bookings? Booking { get; set; }
    }
}
