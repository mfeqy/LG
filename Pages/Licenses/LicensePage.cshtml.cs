using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Youxel.Check.LicensesGenerator.Pages.Licenses
{

    public class LicensePageModel : PageModel
    {
        [BindProperty]
        public string MachineID { get; set; } = default!;
        [BindProperty]
        public string DatabaseName { get; set; } = default!;
        [BindProperty]
        public string ServerName { get; set; } = default!;
        [BindProperty]
        public string DatabaseServerName { get; set; } = default!;

        [BindProperty]
        public int NumberOfUsers { get; set; }
        [BindProperty]
        public DateTime ExpirationDate { get; set; }
        [BindProperty]
        public string Module { get; set; } = default!;

        public string LicenseKey { get; private set; } = default!;

        public async Task<IActionResult> OnPostAsync(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please upload a valid JSON file.");
                return Page();
            }

            using var stream = jsonFile.OpenReadStream();
            var data = await JsonSerializer.DeserializeAsync<MachineInfo>(stream);

            if (data == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid JSON content.");
                return Page();
            }

            MachineID = data.MachineID;
            DatabaseName = data.DatabaseName;
            ServerName = data.ServerName;
            DatabaseServerName = data.DatabaseServerName;

            return Page();
        }

        public IActionResult OnPostGenerateLicense()
        {
            if (!ModelState.IsValid)
                return Page();

            LicenseKey = GenerateLicenseKey();

            return RedirectToPage("LicenseResult", new { LicenseKey });
        }

        private string GenerateLicenseKey()
        {
            // License key logic here
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public class MachineInfo
        {
            public string MachineID { get; set; } = default!;
            public string DatabaseName { get; set; } = default!;
            public string ServerName { get; set; } = default!;
            public string DatabaseServerName { get; set; } = default!;
        }
    }
}