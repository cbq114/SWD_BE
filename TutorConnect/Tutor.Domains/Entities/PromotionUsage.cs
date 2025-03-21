using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class PromotionUsage
    {
        public int promotionId { get; set; }
        public int bookingId { get; set; }
        public decimal DiscountAmount { get; set; }

        [ForeignKey("promotionId")]
        public Promotions promotion { get; set; }
        [ForeignKey("bookingId")]
        public Bookings booking { get; set; }
    }
}
