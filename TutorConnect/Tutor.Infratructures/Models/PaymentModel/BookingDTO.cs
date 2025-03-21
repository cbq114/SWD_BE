using Tutor.Domains.Enums;

namespace Tutor.Infratructures.Models.PaymentModel
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public string customer { get; set; }
        public string Title { get; set; }
        public string? Language { get; set; }
        public string? Level { get; set; }
        public string Instructor { get; set; }
        public DateTime? StartTime { get; set; }
        public double? Price { get; set; }
        public BookingStatus Status { get; set; }
        public string Note { get; set; }
        public DateTime? Created { get; set; }
        public string meetingLink { get; set; }
    }
}
