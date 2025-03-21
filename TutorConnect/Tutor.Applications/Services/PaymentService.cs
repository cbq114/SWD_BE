using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Shared.Helper;

namespace Tutor.Applications.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITutorAvailabilitityRepository _tavailabilitityRepository;
        private readonly IRefundRepository _refundRepository;
        private readonly IProfileRepository _profileRepository;
        public PaymentService(IPaymentRepository paymentRepository, IBookingRepository bookingRepository, ITutorAvailabilitityRepository tavailabilitityRepository, IRefundRepository refundRepository, IProfileRepository profileRepository)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _tavailabilitityRepository = tavailabilitityRepository;
            _refundRepository = refundRepository;
            _profileRepository = profileRepository;
        }

        public async Task<List<PaymentDTO>> GetAllPaymentOfUser(string username)
        {
            return await _paymentRepository.GetAllPaymentOfUser(username);
        }

        public async Task<InvoiceDTO> GetInvoice(int bookingId, string userName)
        {
            // Lấy thông tin booking
            var booking = await _bookingRepository.GetBookingById(bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            // Get original price
            var profile = await _profileRepository.GetProfileByUsername(booking.TutorAvailability.Instructor);
            if (profile == null)
                throw new Exception($"Can not found profile with username: {booking.TutorAvailability.Instructor}");

            // Tạo hóa đơn
            var invoice = new InvoiceDTO
            {
                BookingId = booking.BookingId,
                UserName = userName,
                LessonTitle = booking.Lesson.Title,
                OriginAmount = (double)profile.Price,
                FinalAmount = booking.Price ?? 0,
                StartTime = booking.TutorAvailability.StartTime,
                EndTime = booking.TutorAvailability.EndTime,
            };

            return invoice;
        }

        public async Task<List<Refunds>> GetRefundRequest()
        {
            return await _refundRepository.GetRefundRequest();
        }

        public async Task<Refunds> GetRefundById(int refundId)
        {
            return await _refundRepository.GetRefundsById(refundId);
        }

        public async Task AutoRejectPayment()
        {
            var now = DateTimeHelper.GetVietnamNow();
            var tenMinutesAgo = now.AddMinutes(-10);

            var expiredPayments = await _paymentRepository.GetAllPendingPayment();
            if (!expiredPayments.Any())
                return;

            foreach (var payment in expiredPayments)
            {
                if (payment.CreatedDate <= tenMinutesAgo)
                {
                    await _paymentRepository.ChangePaymentStatus(payment.PaymentId, PaymentStatus.Failed);
                    await _tavailabilitityRepository.ChangeTutorAvailStatus(payment.Booking.AvailabilityId, TutorAvailabilitityStatus.Available);
                }
            }
        }

    }
}
