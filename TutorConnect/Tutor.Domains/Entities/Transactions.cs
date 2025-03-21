using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public int walletId { get; set; }
        public string? OrderCode { get; set; }

        public DateTime CreatedDate { get; set; }
        public double? Amount { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        [ForeignKey("walletId")]
        public Wallet? Wallet { get; set; }
    }
}
