using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using Youxel.Check.LicensesGenerator.Infrastructure.Repositories;

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

        [BindProperty]
        public string MachineKeyString { get; set; } // Stores the array as a string

        public IActionResult OnGetAsync(Guid id)
        {
            License = _licensesRepository.GetLicenseById(id);

            if (License == null)
                return NotFound();

            // Convert MachineKey array to a comma-separated string for UI display
            MachineKeyString = string.Join(", ", License.MachineKey);

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();
            // Convert MachineKey string back into an array
            License.MachineKey = MachineKeyString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(x => x.Trim()).ToArray();

            _licensesRepository.UpdateLicense(License);
            

            return RedirectToPage("Index");
        }
    }
}
