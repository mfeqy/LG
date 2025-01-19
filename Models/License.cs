using Youxel.Check.LicensesGenerator.Enums;

namespace Youxel.Check.LicensesGenerator.Models;

public class License
{
    public LicenseModule Module { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int? NumberOfAdminUsers { get; set; }
    public int? NumberOfUnitUsers { get; set; }
    public int? NumberOfLocationUsers { get; set; }
    public int? NumberOfEndUsers { get; set; }
    public int NumberOfUsers { get; set; }
    public int NumberOfLocations { get; set; }
    public int NumberOfAssets { get; set; }
    public string ExpiryDate { get; set; } = string.Empty;
    public string DatabaseServer { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public string[] MachineKey { get; set; } = Array.Empty<string>();
}
