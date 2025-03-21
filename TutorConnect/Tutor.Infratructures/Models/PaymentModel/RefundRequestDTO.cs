using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class RefundRequestDTO
    {
        public int BookingId { get; set; }
        public string Reason { get; set; }
    }
}
