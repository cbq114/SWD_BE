using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class PromotionUsageRepository : Repository<PromotionUsage>, IPromotionUsageRepository
    {
        private readonly TutorDBContext _dbContext;
        public PromotionUsageRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Create(PromotionUsage promotionUsage)
        {
            await Entities.AddAsync(promotionUsage);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<PromotionUsage>> GetAllOfUser(string username)
        {
            var list = await Entities
                .Include(pu => pu.promotion)
                .Where(pu => pu.promotion.Instructor == username)
                .ToListAsync();
            return list;
        }
    }
}
