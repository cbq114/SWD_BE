using AutoMapper;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Shared.Helper;

namespace Tutor.Applications.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IBookingRepository _bookRepository;
        private readonly ITutorAvailabilitityRepository _tutorAvailabilitityRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        public BookingService(IUserRepository userRepository, ILessonRepository lessonRepository, IBookingRepository bookRepository, ITutorAvailabilitityRepository tutorAvailabilitityRepository, IProfileRepository profileRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _lessonRepository = lessonRepository;
            _bookRepository = bookRepository;
            _tutorAvailabilitityRepository = tutorAvailabilitityRepository;
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<int?> CreateBooking(CreateBookingDTO bookingDTO)
        {

            // Kiểm tra bài học có tồn tại không
            var lesson = await _lessonRepository.GetLessonById(bookingDTO.LessonId);
            if (lesson == null)
                return -1;

            // Kiểm tra lịch dạy của giáo viên có tồn tại không
            var availability = await _tutorAvailabilitityRepository.GetScheduleById(bookingDTO.AvailabilityId);
            if (availability == null || availability.Status == TutorAvailabilitityStatus.Unavailable || availability.Status == TutorAvailabilitityStatus.booked)
                return -2;

            // Kiểm tra nếu bài học có parent lesson
            if (lesson.ParentLessonId.HasValue)
            {
                bool hasCompletedParent = await _bookRepository.CheckUserUsedTolearnedLesson(bookingDTO.Customer, lesson.ParentLessonId.Value);
                if (!hasCompletedParent)
                    return -3;
            }


            // Tạo booking mới
            var tutor = await _userRepository.GetCurrentUser(lesson.Instructor);
            if (tutor == null)
                return -4;

            var booking = new Bookings
            {
                customer = bookingDTO.Customer,
                LessonId = bookingDTO.LessonId,
                AvailabilityId = bookingDTO.AvailabilityId,
                Price = Convert.ToDouble(tutor.Profile.Price),
                Status = BookingStatus.Pending,
                Note = lesson.Title + " " + lesson.Description,
                Created = DateTimeHelper.GetVietnamNow()
            };

            await _bookRepository.CreateBooking(booking);
            return booking.BookingId;
        }

        public async Task AutoRejectPendingBookings()
        {
            var now = DateTimeHelper.GetVietnamNow();
            var tenMinutesAgo = now.AddMinutes(-10);

            var expiredBookings = await _bookRepository.GetAllPendingBooking();

            foreach (var booking in expiredBookings)
            {
                if (booking.Created <= tenMinutesAgo)
                    await _bookRepository.ChangeBookingStatus(booking.BookingId, BookingStatus.Rejected);
            }
        }

        public async Task<List<BookingDTO>> GetAllBookingOfUser(string username)
        {
            var bookingList = await _bookRepository.GetAllBookingOfUser(username);
            List<BookingDTO> result = new List<BookingDTO>();

            foreach (var booking in bookingList)
            {
                var bookDTO = _mapper.Map<BookingDTO>(booking);
                bookDTO.Level = booking.Lesson.Level.ToString();
                bookDTO.Language = await _profileRepository.GetLanguageOfTutor(booking.TutorAvailability.Instructor);
                result.Add(bookDTO);
            }

            return result;
        }

        public async Task<List<BookingDTO>> GetAllBookingTutor(string username)
        {
            var bookingList = await _bookRepository.GetAllBookingTutor(username);
            List<BookingDTO> result = new List<BookingDTO>();

            foreach (var booking in bookingList)
            {
                result.Add(_mapper.Map<BookingDTO>(booking));
            }

            return result;
        }

        public async Task<BookingDTO> GetBookingById(int bookingId)
        {
            var booking = await _bookRepository.GetBookingById(bookingId);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<bool> CancelBooking(int bookingId)
        {
            var booking = await _bookRepository.GetBookingById(bookingId);
            if (booking.Status != BookingStatus.Pending)
                return false;
            await _bookRepository.ChangeBookingStatus(bookingId, BookingStatus.Cancelled);
            return true;
        }

        public async Task<bool> CheckUserUsedTolearnedTutor(string username, string tutor)
        {
            return await _bookRepository.CheckUserUsedTolearnedTutor(username, tutor);
        }

        public Task<bool> CheckUserUsedTolearnedLesson(string username, int lessonId)
        {
            throw new NotImplementedException();
        }
    }
}
