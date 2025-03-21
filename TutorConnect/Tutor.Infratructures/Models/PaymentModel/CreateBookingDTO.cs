using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class CreateBookingDTO
    {
        public string? Customer { get; set; }
        public int LessonId { get; set; }
        public int AvailabilityId { get; set; }
    }
}
