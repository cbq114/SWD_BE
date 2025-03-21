using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Applications.Interfaces
{
    public interface IPromotionService
    {
        Task<string> CreatePromotion(CreatePromotion promo);
        Task<PromotionDTO> GetById(int promotionId);
        Task<PromotionDTO> GetByCode(string code, int bookingId);
        Task<List<PromotionDTO>> GetAllPromotionOfTutor(string username);
        Task<string> DeletePromotion(string tutor, int promotionId);
        Task<PromotionDTO> UpdatePromotion(PromotionDTO promotion);
        Task<Promotions?> GetByTutorAndCode(string tutor, string code);
        Task<bool> CheckDuplicateCodeOfTutor(string tutor, string code);

        //Task<List<PromotionDTO>> GetPromotionByLessionOfTutor(string username, int lessionId);
    }
}
