using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using Youxel.Check.LicensesGenerator.Domain.Enums;
using Youxel.Check.LicensesGenerator.Domain.Models;
using Youxel.Check.LicensesGenerator.Infrastructure.Repositories;
using Youxel.Check.LicensesGenerator.Utilities.Helpers;

namespace Youxel.Check.LicensesGenerator.Pages.Licenses
{

    public class LicensePageModel : PageModel
    {
        private readonly ILicensesRepository _licenseRepository;
        public LicensePageModel(ILicensesRepository licenseRepository)
        {
            _licenseRepository = licenseRepository;
        }

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

            var licenseRequest = LicenseHelper.GetLicense(fileContent);
            MachineKey = licenseRequest.MachineKey;
            DatabaseName = licenseRequest.DatabaseName;
            DatabaseServer = licenseRequest.DatabaseServer;
            Module = licenseRequest.Module;

            return Page();
        }

        public async Task<IActionResult> OnPostGenerateLicense()
        {
            if (!ModelState.IsValid)
                return Page();

            var licenseVm = new LicenseVM
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

            var licenseJson = JsonSerializer.Serialize(licenseVm);
            var licenseKey = LicenseHelper.GenerateLicenseKey(licenseJson);

            await _licenseRepository.CreateLicenseAsync(License.Create(licenseVm, licenseKey));

            return RedirectToPage("LicenseResult", new { licenseKey });
        }
    }
}