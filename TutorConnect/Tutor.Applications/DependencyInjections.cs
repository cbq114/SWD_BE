using Microsoft.Extensions.DependencyInjection;
using Tutor.Applications.BackgroundServices;
using Tutor.Applications.Interfaces;
using Tutor.Applications.Services;

namespace Tutor.Applications
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthenService, AuthenService>();
            services.AddScoped<ICloundinaryService, CloundinaryService>();
            services.AddScoped<IUpgradeRequestService, UpgradeRequestService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICertificationService, CertificationService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ITutorScheduleService, TutorScheduleService>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IPaypalService, PaypalService>();
            services.AddScoped<ITransactionService, TransactionsService>();
            services.AddHostedService<AutoRejectBookingService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IRatePriceService, RatePriceService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IFeedbackService, FeedbacksService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddHostedService<AttendanceAutomationService>();
            return services;
        }
    }
}