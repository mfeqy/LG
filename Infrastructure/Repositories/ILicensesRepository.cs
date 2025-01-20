using Youxel.Check.LicensesGenerator.Domain.Entities;

namespace Youxel.Check.LicensesGenerator.Infrastructure.Repositories
{
    public interface ILicensesRepository
    {
        List<License> GetLicenses();
        License GetLicenseById(Guid id);
        Task<License> CreateLicenseAsync(License license);
        void UpdateLicense(License license);
    }
}
