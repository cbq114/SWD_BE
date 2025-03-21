using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Applications.Interfaces
{
    public interface IPaymentService
    {
        Task<InvoiceDTO> GetInvoice(int bookingId, string userName);
        Task<List<PaymentDTO>> GetAllPaymentOfUser(string username);
        Task<List<Refunds>> GetRefundRequest();
        Task<Refunds> GetRefundById(int refundId);
        Task AutoRejectPayment();
    }
}
