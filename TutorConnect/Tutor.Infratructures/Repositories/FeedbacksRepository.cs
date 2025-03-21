using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    internal class FeedbacksRepository : Repository<Feedbacks>, IFeedbacksRepository
    {
        private readonly TutorDBContext _dbContext;
        public FeedbacksRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Feedbacks> GetFeedbackById(int id)
        {
            var feedback = await Entities
                .Include(f => f.Booking)
                    .ThenInclude(b => b.TutorAvailability)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
            return feedback;
        }

        public async Task<bool> AddFeedBack(Feedbacks createFeedback)
        {
            if (createFeedback == null)
                return false;
            await _dbContext.Feedbacks.AddAsync(createFeedback);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Feedbacks>> GetAllApprovedFeedBacks(string tutor)
        {
            var feedbackList = await Entities
                .Include(f => f.Booking).ThenInclude(b => b.TutorAvailability)
                .Where(f => f.Booking.TutorAvailability.Instructor.Equals(tutor) && f.Status == FeedbackStatus.Approved)
                .ToListAsync();
            return feedbackList;
        }

        public async Task<List<Feedbacks>> GetAllFeedBacks(string tutor)
        {
            var feedbackList = await Entities
                .Include(f => f.Booking).ThenInclude(b => b.TutorAvailability)
                .Where(f => f.Booking.TutorAvailability.Instructor.Equals(tutor))
                .ToListAsync();
            return feedbackList;
        }

        public async Task<Feedbacks> GetUserFeedbackOfTutor(string username, string tutor)
        {
            var feedback = await Entities
                .Include(f => f.Booking)
                    .ThenInclude(b => b.TutorAvailability)
                .FirstOrDefaultAsync(f => f.username == username && f.Booking.TutorAvailability.Instructor == tutor);
            return feedback;
        }

        public async Task<bool> ChangeFeedbackStatus(int feedbackId, FeedbackStatus status)
        {
            var feedback = await GetFeedbackById(feedbackId);
            if (feedback == null)
                return false;

            feedback.Status = status;
            Entities.Update(feedback);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CheckUserUsedToFeedbackOnBooking(int bookingId)
        {
            var feedback = await Entities.FirstOrDefaultAsync(f => f.BookingId ==  bookingId);
            if (feedback == null)
                return false;
            return true;
        }
    }
}
