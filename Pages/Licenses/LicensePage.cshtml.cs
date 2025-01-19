using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using Youxel.Check.LicensesGenerator.Enums;
using Youxel.Check.LicensesGenerator.Helpers;
using Youxel.Check.LicensesGenerator.Models;

namespace Youxel.Check.LicensesGenerator.Pages.Licenses
{

    public class LicensePageModel : PageModel
    {
        [BindProperty]
        public LicenseModule Module { get; set; } = default!;

        [BindProperty]
        public string CompanyName { get; set; } = string.Empty;

        [BindProperty]
        public int? NumberOfAdminUsers { get; set; }

        [BindProperty]
        public int? NumberOfUnitUsers { get; set; }

        [BindProperty]
        public int? NumberOfLocationUsers { get; set; }

        [BindProperty]
        public int? NumberOfEndUsers { get; set; }

        [BindProperty]
        public int NumberOfUsers { get; set; }

        [BindProperty]
        public int NumberOfLocations { get; set; }

        [BindProperty]
        public int NumberOfAssets { get; set; }

        [BindProperty]
        public string ExpiryDate { get; set; } = string.Empty;

        [BindProperty]
        public string DatabaseServer { get; set; } = string.Empty;

        [BindProperty]
        public string DatabaseName { get; set; } = string.Empty;

        [BindProperty]
        public string[] MachineKey { get; set; } = Array.Empty<string>();

        public async Task<IActionResult> OnPostAsync(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid JSON file.");
                return Page();
            }

            string fileContent;
            using (var stream = new MemoryStream())
            {
                await jsonFile.CopyToAsync(stream);
                var fileBytes = stream.ToArray();
                fileContent = Encoding.UTF8.GetString(fileBytes);
            }

            if (fileContent == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid JSON content.");
                return Page();
            }

            License licenseRequest = LicenseHelper.GetLicense(fileContent);
            MachineKey = licenseRequest.MachineKey;
            DatabaseName = licenseRequest.DatabaseName;
            DatabaseServer = licenseRequest.DatabaseServer;
            Module = licenseRequest.Module;

            return Page();
        }

        public IActionResult OnPostGenerateLicense()
        {
            if (!ModelState.IsValid)
                return Page();

            var license = new License
            {
                Module = Module,
                CompanyName = CompanyName,
                NumberOfAdminUsers = NumberOfAdminUsers,
                NumberOfUnitUsers = NumberOfUnitUsers,
                NumberOfLocationUsers = NumberOfLocationUsers,
                NumberOfEndUsers = NumberOfEndUsers,
                NumberOfUsers = NumberOfUsers,
                NumberOfLocations = NumberOfLocations,
                NumberOfAssets = NumberOfAssets,
                ExpiryDate = ExpiryDate,
                DatabaseServer = DatabaseServer,
                DatabaseName = DatabaseName,
                CreationDate = DateTime.Now,
                MachineKey = MachineKey
            };

            var licenseJson = JsonSerializer.Serialize(license);
            var licenseKey = LicenseHelper.GenerateLicenseKey(licenseJson);

            return RedirectToPage("LicenseResult", new { licenseKey });
        }
    }
}