using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using System.Text.Json;

namespace Youxel.Check.LicensesGenerator.Infrastructure.EntityConfigurations
{
    public class LicenseEntityConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(l => l.MachineKey)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                   v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions)null)
               );
        }
    }
}
