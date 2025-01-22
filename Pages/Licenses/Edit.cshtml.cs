using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using Youxel.Check.LicensesGenerator.Domain.Models;
using Youxel.Check.LicensesGenerator.Infrastructure.Repositories;
using Youxel.Check.LicensesGenerator.Utilities.Helpers;

namespace Youxel.Check.LicensesGenerator.Pages.Licenses
{
    public class EditModel : PageModel
    {
        private readonly ILicensesRepository _licensesRepository;

        public EditModel(ILicensesRepository licensesRepository)
        {
            _licensesRepository = licensesRepository;
        }

        [BindProperty]
        public License License { get; set; }

        public IActionResult OnGetAsync(Guid id)
        {
            License = _licensesRepository.GetLicenseById(id);

            if (License == null)
                return NotFound();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var licenseVm = new LicenseVM
            {
                Module = License.Module,
                CompanyName = License.CompanyName,
                NumberOfAdminUsers = License.NumberOfAdminUsers,
                NumberOfUnitUsers = License.NumberOfUnitUsers,
                NumberOfLocationUsers = License.NumberOfLocationUsers,
                NumberOfEndUsers = License.NumberOfEndUsers,
                NumberOfUsers = License.NumberOfUsers,  // This is now dynamically calculated
                NumberOfLocations = License.NumberOfLocations,
                NumberOfAssets = License.NumberOfAssets,
                ExpiryDate = License.ExpiryDate.ToString(),
                DatabaseServer = License.DatabaseServer,
                DatabaseName = License.DatabaseName,
                CreationDate = DateTime.Now,
                MachineKey = License.MachineKey
            };

            var licenseJson = JsonSerializer.Serialize(licenseVm);
            var licenseKey = LicenseHelper.GenerateLicenseKey(licenseJson);
            License.LicenseKey = licenseKey;

            _licensesRepository.UpdateLicense(License);


            return RedirectToPage("LicenseResult", new { licenseKey });
        }
    }
}
