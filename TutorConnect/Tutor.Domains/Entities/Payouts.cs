using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tutor.Domains.Enums;

namespace Tutor.Domains.Entities
{
    public class Payouts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayoutId { get; set; }

        public double Amount { get; set; }

        public DateTime PayoutDate { get; set; }

        public PayoutStatus Status { get; set; }
        public string? Reason { get; set; }

        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public Users User { get; set; }
        public virtual ICollection<PayoutResponses> PayoutResponses { get; set; }
    }
}
