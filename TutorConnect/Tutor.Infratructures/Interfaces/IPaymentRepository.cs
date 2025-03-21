using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Models.PaymentModel;

namespace Tutor.Infratructures.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payments> GetPaymentById(int id);
        Task<Payments> GetSuccessPaymentById(int id);
        Task<List<PaymentDTO>> GetAllPaymentOfUser(string username);
        Task<List<Payments>> GetAllPendingPayment();
        Task CreatePayment(Payments payments);
        Task ChangePaymentStatus(int id, PaymentStatus status);
        Task<Payments> GetPaymentByBookingId(int bookingId);
        Task<Payments> GetByBookingId(int bookingId);
        Task UpdatePayment(Payments payment);
        Task<List<Payments>> GetAllNotPaidSuccessPayment(DateTime thresholdTime);
    }
}
