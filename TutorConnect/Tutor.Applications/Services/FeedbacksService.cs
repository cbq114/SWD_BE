using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Applications.Interfaces;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Services
{
    public class FeedbacksService : IFeedbackService
    {
        private readonly IFeedbacksRepository _feedbacksRepository;
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;

        public FeedbacksService(IFeedbacksRepository feedbacksRepository, IMapper mapper, IBookingRepository bookingRepository)
        {
            _feedbacksRepository = feedbacksRepository;
            _mapper = mapper;
            _bookingRepository = bookingRepository;
        }
        public async Task<string> CreateFeedback(CreateFeedback createFeedback, string username)
        {
            var booking = await _bookingRepository.GetBookingById(createFeedback.BookingId);
            if (booking == null)
                return $"Error: Can not find booking with this id: {createFeedback.BookingId}";

            if (!booking.customer.Equals(username))
                return "Error: You can can only give feedback by your booking";

            if (booking.Status != Domains.Enums.BookingStatus.Completed)
                return $"Error: Your can only give feedback after completed your lesson";

            var feedback = _mapper.Map<Feedbacks>(createFeedback);
            feedback.Status = Domains.Enums.FeedbackStatus.Approved;
            feedback.username = username;
            var created = await _feedbacksRepository.AddFeedBack(feedback);
            if (!created)
                return "Error: Cannot create feedback!";
            return "Feedback create successfully.";
        }

        public async Task<List<FeedbacksDTO>> GetAllFeedbacksOf(string tutor)
        {
            var feebackList = await _feedbacksRepository.GetAllApprovedFeedBacks(tutor);
            if (!feebackList.Any())
                return null;

            return _mapper.Map<List<FeedbacksDTO>>(feebackList);
        }

        public async Task<List<FeedbacksDTO>> GetAllFeedBacks(string username)
        {
            var feebackList = await _feedbacksRepository.GetAllFeedBacks(username);
            if (!feebackList.Any())
                return null;

            return _mapper.Map<List<FeedbacksDTO>>(feebackList);
        }

        public async Task<string> HideFeedback(string username, int feedbackId)
        {
            var feedback = await _feedbacksRepository.GetFeedbackById(feedbackId);
            if (feedback.Status == Domains.Enums.FeedbackStatus.Rejected)
                return "Error: This feedback is already hided.";

            if (!feedback.Booking.TutorAvailability.Instructor.Equals(username))
                return "Error: You dont have permission to chang orthers feedback's tutor!";

            var changed = await _feedbacksRepository.ChangeFeedbackStatus(feedbackId, Domains.Enums.FeedbackStatus.Rejected);
            if (!changed)
                return "Error: Error while change feedback status!";

            return "Feedback is now hide on your profile";
        }

        public async Task<FeedbacksDTO> GetUserFeedbackOfTutor(string username, string tutor)
        {
            var feedback = await _feedbacksRepository.GetUserFeedbackOfTutor(username, tutor);
            return _mapper.Map<FeedbacksDTO>(feedback);
        }

        public async Task<bool> CheckUserUsedToFeedbackOnBooking(int bookingId)
        {
            return await _feedbacksRepository.CheckUserUsedToFeedbackOnBooking(bookingId);
        }
    }
}
