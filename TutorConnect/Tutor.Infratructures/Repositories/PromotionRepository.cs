using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
    public class PromotionRepository : Repository<Promotions>, IPromotionRepository
    {
        private readonly TutorDBContext _dbContext;

        public PromotionRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreatePromotion(Promotions promo)
        {
            var result = await Entities.AddAsync(promo);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Promotions?> GetById(int promotionId)
        {
            return await Entities.FirstOrDefaultAsync(p => p.promotionId == promotionId && p.Status == PromotionStatus.Active);
        }

        public async Task<Promotions?> GetByCode(string code, string tutorname)
        {
            return await Entities.FirstOrDefaultAsync(p => p.Code == code && p.Instructor == tutorname && p.Status == PromotionStatus.Active);
        }

        public async Task<List<Promotions>> GetAllPromotionOfTutor(string username)
        {
            var promos = await Entities
                .Include(p => p.Lesson)
                .Where(p => p.Instructor == username && p.Lesson.Status == LessonStatus.Active && p.Status == PromotionStatus.Active)
                .ToListAsync();
            return promos;
        }

        //public async Task<List<Promotions>> GetPromotionByLessionOfTutor(string username, int lessionId)
        //{
        //    var promos = await Entities
        //        .Include(p => p.Lesson)
        //        .Where(p => p.Instructor == username 
        //            && p.Lesson.Status == LessonStatus.Active 
        //            && p.LessonId == lessionId
        //            && p.Status == PromotionStatus.Active)
        //        .ToListAsync();
        //    return promos;
        //}

        public async Task<bool> DeletePromotion(int promotionId)
        {
            var promo = await Entities.FindAsync(promotionId);
            promo.Status = PromotionStatus.Inactive;
            Entities.Update(promo);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Promotions> UpdatePromotion(Promotions promotion)
        {
            Entities.Update(promotion);
            await _dbContext.SaveChangesAsync();
            return promotion;
        }

        public async Task<bool> UsePromotion(int promotionId)
        {
            var promo = await Entities.FirstOrDefaultAsync(p => p.promotionId == promotionId);
            promo.limit -= 1;
            Entities.Update(promo);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Promotions?> GetByTutorAndCode(string tutor, string code)
        {
            return await Entities.FirstOrDefaultAsync(p => p.Instructor == tutor && p.Code == code);
        }
    }
}
