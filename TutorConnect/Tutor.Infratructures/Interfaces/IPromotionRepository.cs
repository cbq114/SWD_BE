using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface IPromotionRepository
    {
        Task<bool> CreatePromotion(Promotions promo);
        Task<Promotions> GetById(int promotionId);
        Task<Promotions> GetByCode(string code, string tutorname);
        Task<List<Promotions>> GetAllPromotionOfTutor(string username);
        //Task<List<Promotions>> GetPromotionByLessionOfTutor(string username, int lessionId);
        Task<bool> DeletePromotion(int promotionId);
        Task<Promotions> UpdatePromotion(Promotions promotion);
        Task<bool> UsePromotion(int promotionId);
        Task<Promotions?> GetByTutorAndCode(string tutor, string code);
    }
}
