namespace Tutor.Infratructures.Models.DashboardModel
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public string CustomerUserName { get; set; }
        public string CustomerFullName { get; set; }
        public string LessonTitle { get; set; }
        public string InstructorUserName { get; set; }
        public DateTime? BookingDate { get; set; }
        public double? Price { get; set; }
        public string Status { get; set; }
    }
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class RefundViewModel
    {
        public int RefundId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime RefundDate { get; set; }
    }

    public class PayoutViewModel
    {
        public int PayoutId { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PayoutDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
    }
    public class FeedbackViewModel
    {
        public int FeedbackId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
