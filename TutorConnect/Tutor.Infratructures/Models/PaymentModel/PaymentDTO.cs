using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string? PaymentCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public double? Amount { get; set; }
        public string? Description { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
