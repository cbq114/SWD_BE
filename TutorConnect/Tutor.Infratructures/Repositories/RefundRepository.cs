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
    public class RefundRepository : Repository<Refunds>, IRefundRepository
    {
        private readonly TutorDBContext _dbContext;
        public RefundRepository(TutorDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateRefund(Refunds refunds)
        {
            await Entities.AddAsync(refunds);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Refunds> GetRefundsById(int refundId)
        {
            var refund = await Entities.FindAsync(refundId);    
            return refund;
        }

        public async Task<bool> UpdateRefund(Refunds refund)
        {
            Entities.Update(refund);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<Refunds>> GetRefundRequest()
        {
            return await Entities.ToListAsync();
        }
    }
}
