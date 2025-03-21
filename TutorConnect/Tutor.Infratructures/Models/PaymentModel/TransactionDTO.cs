using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class TransactionDTO
    {
        public int TransactionId { get; set; }
        public int walletId { get; set; }
        public string? OrderCode { get; set; }

        public DateTime CreatedDate { get; set; }
        public double? Amount { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? UserName { get; set; }

    }
}
