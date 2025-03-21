using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.LessonModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface IFeedbacksRepository
    {
        Task<Feedbacks> GetFeedbackById(int id);
        Task<List<Feedbacks>> GetAllFeedBacks (string tutor);
        Task<List<Feedbacks>> GetAllApprovedFeedBacks(string tutor);
        Task<bool> AddFeedBack(Feedbacks createFeedback);
        Task<Feedbacks> GetUserFeedbackOfTutor(string username, string tutor);
        Task<bool> ChangeFeedbackStatus(int feedbackId, FeedbackStatus status);
        Task<bool> CheckUserUsedToFeedbackOnBooking(int bookingId);
    }
}
