using Microsoft.EntityFrameworkCore;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using Youxel.Check.LicensesGenerator.Infrastructure.Data;

namespace Youxel.Check.LicensesGenerator.Infrastructure.Repositories
{
    public class LicensesRepository : ILicensesRepository
    {
        private readonly LicenseDbContext _context;
        public LicensesRepository(LicenseDbContext context)
        {
            _context = context;
        }

        public List<License> GetLicenses()
        {
            return _context.Licenses.AsNoTracking().ToList();
        }

        public License GetLicenseById(Guid id)
        {
            return _context.Licenses.Find(id);
        }

        public async Task<License> CreateLicenseAsync(License license)
        {
            _context.Licenses.Add(license);
            await _context.SaveChangesAsync();
            return license;
        }

        public void UpdateLicense(License license)
        {
            _context.Licenses.Update(license);
            _context.SaveChanges();
        }
    }
}
