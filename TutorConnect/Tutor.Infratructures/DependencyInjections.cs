using Microsoft.Extensions.DependencyInjection;
using Tutor.Infratructures.Configurations;
using Tutor.Infratructures.Interfaces;
using Tutor.Infratructures.Mapper;
using Tutor.Infratructures.Repositories;

namespace Tutor.Infratructures
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfratructure(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddTransient(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAuthenRepository, AuthenRepository>();
            services.AddScoped<IEmailSender, EmailSenderRepository>();
            services.AddScoped<ICloundinaryRepository, CloundinaryRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteInstructorRepository>();
            services.AddScoped<IUpgradeRequestRepository, UpgradeRequestRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITutorAvailabilitityRepository, TutorAvailabilitityRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<ICertificationRepository, CertificationRepository>();
            services.AddScoped<IHomeRepository, HomeRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteInstructorRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IRefundRepository, RefundRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IFeedbacksRepository, FeedbacksRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPromotionRepository, PromotionRepository>();
            services.AddScoped<IPromotionUsageRepository, PromotionUsageRepository>();
            services.AddScoped<ILessonAttendanceDetailsRepository, LessonAttendanceDetailsRepository>();
            services.AddScoped<ILessonAttendanceRepository, LessonAttendanceRepository>();
            return services;
        }
    }
}
