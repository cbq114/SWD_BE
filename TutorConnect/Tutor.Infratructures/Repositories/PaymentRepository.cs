using AutoMapper;
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
using Tutor.Infratructures.Models.PaymentModel;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class PaymentRepository : Repository<Payments>, IPaymentRepository
    {
        private readonly IMapper _mapper;
        public PaymentRepository(TutorDBContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<Payments> GetPaymentById(int id)
        {
            var payment = await Entities
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
            if (payment == null)
                throw new ArgumentNullException(nameof(payment), $"Can not find booking with id = {id}");

            return payment;
        }

        public async Task<Payments> GetSuccessPaymentById(int id)
        {
            var payment = await Entities
                .Include(p => p.Booking)
                .ThenInclude(b => b.TutorAvailability)
                .FirstOrDefaultAsync(p => p.PaymentId == id && p.Status == PaymentStatus.Success);
            if (payment == null)
                Console.WriteLine( new ArgumentNullException(nameof(payment), $"Can not find success booking with id = {id}"));

            return payment;
        }

        public async Task CreatePayment(Payments payments)
        {
            await base.Add(payments);
        }

        public async Task ChangePaymentStatus(int id, PaymentStatus status)
        {
            var payment = await GetPaymentById(id);
            payment.Status = status;
        }

        public async Task<Payments> GetPaymentByBookingId(int bookingId)
        {
            return await Entities.FirstOrDefaultAsync(p => p.BookingId == bookingId && p.Status == PaymentStatus.Pending);
        }

        public async Task<Payments> GetByBookingId(int bookingId)
        {
            return await Entities
                .Include(p => p.Booking)
                    .ThenInclude(b => b.TutorAvailability)
                .FirstOrDefaultAsync(p => p.BookingId == bookingId);
        }

        public async Task UpdatePayment(Payments payment)
        {
            await base.Update(payment);
        }

        public async Task<List<PaymentDTO>> GetAllPaymentOfUser(string username)
        {
            var payments = await Entities
                .Include(p => p.Booking)
                .Where(p => p.Booking.customer == username)
                .ToListAsync();
            return payments.Any() ? _mapper.Map<List<PaymentDTO>>(payments) : [];
        }

        public async Task<List<Payments>> GetAllPendingPayment()
        {
            var payments = await Entities
                .Include(p => p.Booking)
                .Where(p => p.Status == PaymentStatus.Pending)
                .ToListAsync();
            return payments ?? [];
        }

        public async Task<List<Payments>> GetAllNotPaidSuccessPayment(DateTime thresholdTime)
        {
            return await Entities
                .Include(p => p.Booking).ThenInclude(b => b.TutorAvailability)
                .Where(p => p.CreatedDate <= thresholdTime && p.Status == PaymentStatus.Success && p.IsPaid == false)
                .ToListAsync();
        }
    }
}
