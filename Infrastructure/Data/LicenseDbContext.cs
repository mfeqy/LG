using Microsoft.EntityFrameworkCore;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using Youxel.Check.LicensesGenerator.Infrastructure.EntityConfigurations;

namespace Youxel.Check.LicensesGenerator.Infrastructure.Data
{
    public class LicenseDbContext : DbContext
    {
        public LicenseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<License> Licenses { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LicenseEntityConfiguration());
        }
    }
}
