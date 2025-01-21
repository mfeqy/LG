using Youxel.Check.LicensesGenerator.Infrastructure.Data;

namespace Youxel.Check.LicensesGenerator.Infrastructure.Repositories
{
    public class AppSettingsRepository : IAppSettingsRepository
    {
        private readonly LicenseDbContext _context;

        public AppSettingsRepository(LicenseDbContext context)
        {
            _context = context;
        }

        public string GetByKey(string key)
        {
            return _context.AppSettings.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
        }
    }
}
