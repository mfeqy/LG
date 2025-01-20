using Youxel.Check.LicensesGenerator.Domain.Enums;
using Youxel.Check.LicensesGenerator.Domain.Models;

namespace Youxel.Check.LicensesGenerator.Domain.Entities
{
    public class License
    {
        public Guid Id { get; set; }
        public string LicenseKey { get; set; }
        public LicenseModule Module { get; set; }
        public string CompanyName { get; set; }
        public int? NumberOfAdminUsers { get; set; }
        public int? NumberOfUnitUsers { get; set; }
        public int? NumberOfLocationUsers { get; set; }
        public int? NumberOfEndUsers { get; set; }
        public int NumberOfUsers { get; set; }
        public int NumberOfLocations { get; set; }
        public int NumberOfAssets { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DatabaseServer { get; set; }
        public string DatabaseName { get; set; }
        public DateTime CreationDate { get; set; }
        public string[] MachineKey { get; set; }

        public static License Create(LicenseVM model, string licenseKey)
        {
            return new License
            {
                Id = Guid.NewGuid(),
                LicenseKey = licenseKey,
                CreationDate = DateTime.Now,
                Module = model.Module,
                CompanyName = model.CompanyName,
                NumberOfAdminUsers = model.NumberOfAdminUsers,
                NumberOfUnitUsers = model.NumberOfUnitUsers,
                NumberOfLocationUsers = model.NumberOfLocationUsers,
                NumberOfEndUsers = model.NumberOfEndUsers,
                NumberOfUsers = model.NumberOfUsers,
                DatabaseName = model.DatabaseName,
                DatabaseServer = model.DatabaseServer,
                ExpiryDate = DateTime.Parse(model.ExpiryDate),
                NumberOfLocations = model.NumberOfLocations,
                NumberOfAssets = model.NumberOfAssets,
                MachineKey = model.MachineKey,                
            };
        }
    }
}
