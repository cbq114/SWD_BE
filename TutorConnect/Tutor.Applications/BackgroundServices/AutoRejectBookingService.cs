using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutor.Applications.Interfaces;

namespace Tutor.Applications.BackgroundServices
{
    public class AutoRejectBookingService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AutoRejectBookingService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    await bookingService.AutoRejectPendingBookings();
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                    await paymentService.AutoRejectPayment();
                    var walletService = scope.ServiceProvider.GetRequiredService<IWalletService>();
                    await walletService.autoPaidForTutor();
                }

                // Chạy mỗi 10 phút
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
