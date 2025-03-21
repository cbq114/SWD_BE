using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tutor.Infratructures.Persistence
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TutorDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TutorDTB")));

            return services;
        }
    }
}
