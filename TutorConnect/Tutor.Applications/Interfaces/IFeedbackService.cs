using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.UserModel;

namespace Tutor.Applications.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<FeedbacksDTO>> GetAllFeedbacksOf(string tutorname);
        Task<List<FeedbacksDTO>> GetAllFeedBacks(string username);
        Task<string> HideFeedback(string username, int feedbackId);
        Task<string> CreateFeedback(CreateFeedback createFeedback, string username);
        Task<FeedbacksDTO> GetUserFeedbackOfTutor(string username, string tutor);
        Task<bool> CheckUserUsedToFeedbackOnBooking(int bookingId);
    }
}
