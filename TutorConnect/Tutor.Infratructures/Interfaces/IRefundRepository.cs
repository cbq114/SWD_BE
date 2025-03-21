using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;

namespace Tutor.Infratructures.Interfaces
{
    public interface IRefundRepository
    {
        Task CreateRefund(Refunds refunds);
        Task<Refunds> GetRefundsById(int refundId);
        Task<bool> UpdateRefund(Refunds refund);
        Task<List<Refunds>> GetRefundRequest();
    }
}
