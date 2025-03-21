using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Refunds
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundId { get; set; }

        public int BookingId { get; set; }

        public string? TransactionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime RefundDate { get; set; }

        public string Reason { get; set; }

        public RefundStatus Status { get; set; }

        [ForeignKey("BookingId")]
        public Bookings Booking { get; set; }
    }
}
