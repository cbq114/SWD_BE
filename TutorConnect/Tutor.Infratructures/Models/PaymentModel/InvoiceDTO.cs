using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class InvoiceDTO
    {
        public int BookingId { get; set; }
        public string? UserName { get; set; }
        public string LessonTitle { get; set; }
        public double OriginAmount { get; set; }
        public double FinalAmount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
