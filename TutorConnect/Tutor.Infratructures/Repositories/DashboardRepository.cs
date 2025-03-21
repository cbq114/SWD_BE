using Microsoft.EntityFrameworkCore;
using Tutor.Domains.Entities;
using Tutor.Domains.Enums;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Persistence;

namespace Tutor.Infratructures.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly TutorDBContext _context;

        public DashboardRepository(TutorDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bookings>> GetBookingsAsync()
            => await _context.Bookings.Include(b => b.User).Include(b => b.Lesson).ToListAsync();

        public async Task<Bookings> GetBookingByIdAsync(int bookingId)
            => await _context.Bookings.Include(b => b.User).Include(b => b.Lesson)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        public async Task UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            booking.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payments>> GetPaymentsAsync()
            => await _context.Payments.ToListAsync();

        public async Task<IEnumerable<Refunds>> GetRefundsAsync()
            => await _context.Refunds.ToListAsync();

        public async Task<Refunds> GetRefundByIdAsync(int refundId)
            => await _context.Refunds.FindAsync(refundId);

        public async Task<IEnumerable<Payouts>> GetPayoutsAsync()
            => await _context.Payouts.Include(p => p.User).ToListAsync();

        public async Task<IEnumerable<Payouts>> GetPayoutsByUserNameAsync(string username)
    => await _context.Payouts.Include(p => p.User).Where(u => u.UserName == username).ToListAsync();

        public async Task<IEnumerable<Feedbacks>> GetFeedbacksAsync()
            => await _context.Feedbacks.Include(f => f.User).ToListAsync();

        public async Task DeleteFeedbackAsync(int feedbackId)
        {
            var feedback = await _context.Feedbacks.FindAsync(feedbackId);
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<object> GetDashboardSummaryAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalTutors = await _context.Users.CountAsync(u => u.RoleId == 2);
            var totalStudents = await _context.Users.CountAsync(u => u.RoleId == 1);
            var totalRevenue = await _context.Payments.Where(p => p.Status == PaymentStatus.Success).SumAsync(p => p.Amount) ?? 0;
            var pendingPayouts = await _context.Payouts.CountAsync(p => p.Status == PayoutStatus.Pending);
            var systemBalance = await _context.Wallet
                .Include(w => w.User)
                .Where(w => w.User.RoleId == 4)
                .SumAsync(w => w.Balance);

            return new
            {
                totalUsers,
                totalTutors,
                totalStudents,
                totalRevenue,
                pendingPayouts,
                systemBalance
            };
        }

        public async Task<IEnumerable<object>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyRevenue = await _context.Payments
                                    .Where(p => p.Status == PaymentStatus.Success && p.CreatedDate.Year == year)
                                    .GroupBy(p => p.CreatedDate.Month)
                                    .Select(group => new
                                    {
                                        month = group.Key,
                                        revenue = group.Sum(p => p.Amount) ?? 0
                                    })
                                    .OrderBy(g => g.month)
                                    .ToListAsync();

            return monthlyRevenue;
        }

        public async Task<IEnumerable<object>> GetUserDistributionAsync()
        {
            var distribution = await _context.Users
                        .GroupBy(u => u.Role.RoleName)
                        .Select(group => new
                        {
                            name = group.Key,
                            value = group.Count()
                        })
                        .ToListAsync();

            return distribution;
        }

        public async Task<IEnumerable<object>> GetUserGrowthAsync(int year)
        {
            var growth = await _context.Users
                .Where(u => u.CreatedDate.Year == year)
                .GroupBy(u => u.CreatedDate.Month)
                .Select(group => new
                {
                    month = group.Key,
                    students = group.Count(u => u.RoleId == 1),
                    tutors = group.Count(u => u.RoleId == 2)
                })
                .OrderBy(g => g.month)
                .ToListAsync();

            return growth;
        }
    }
}
