using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface IPromotionUsageRepository
    {
        Task<bool> Create(PromotionUsage promotionUsage);
        Task<List<PromotionUsage>> GetAllOfUser(string username);
    }
}
