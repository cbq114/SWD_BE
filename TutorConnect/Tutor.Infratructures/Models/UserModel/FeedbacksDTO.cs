using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.UserModel
{
    public class CreateFeedback
    {
        public int Star { get; set; }
        public string Comment { get; set; }
        public string? username { get; set; }
        public int BookingId { get; set; }
        public FeedbackStatus? Status { get; set; }
    }

    public class  FeedbacksDTO : CreateFeedback
    {
        public int FeedbackId { get; set; }
    }
}
