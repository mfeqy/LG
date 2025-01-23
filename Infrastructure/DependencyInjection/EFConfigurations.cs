using Microsoft.EntityFrameworkCore;
using Youxel.Check.LicensesGenerator.Domain.Consts;
using Youxel.Check.LicensesGenerator.Infrastructure.Data;
using Youxel.Check.LicensesGenerator.Infrastructure.DependencyInjection;
using Youxel.Check.LicensesGenerator.Infrastructure.Repositories;

namespace Youxel.Check.LicensesGenerator.Infrastructure.DependencyInjection
{
    public static class EFConfigurations
    {
        public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LicenseDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")!));
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ILicensesRepository, LicensesRepository>();
            services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();
            return services;
        }

        public static async Task<WebApplication> MigrateAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LicenseDbContext>();
                await dbContext.Database.MigrateAsync();
                Constants.PrivateKey = dbContext.AppSettings.FirstOrDefault(s => s.Key == "PrivateKey").Value;
            }
            return app;
        }
    }
}
