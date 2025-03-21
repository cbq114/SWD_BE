using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class PayoutResponses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayoutResponseId { get; set; }
        public int PayoutId { get; set; }
        public bool IsApproved { get; set; }
        public DateTime ResponseDate { get; set; }
        public string? Comment { get; set; }
        [ForeignKey("PayoutId")]
        public Payouts Payout { get; set; }
    }
}
