using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tutor.Domains.Entities
{
    public class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WalletId { get; set; }

        [Required]
        public double Balance { get; set; } = 0;
        [Required]
        public DateTime TransactionTime { get; set; }
        public string? UserName { get; set; }

        [ForeignKey("UserName")]
        public Users? User { get; set; }

        public virtual ICollection<Transactions>? Transactions { get; set; }
    }
}
