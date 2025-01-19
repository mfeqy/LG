using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Youxel.Check.LicensesGenerator.Pages.Licenses
{
    public class LicenseResultModel : PageModel
    {
        public string LicenseKey { get; set; } = default!;

        public void OnGet(string licenseKey)
        {
            if (string.IsNullOrEmpty(licenseKey))
            {
                LicenseKey = "Key not generated!";
            }

            LicenseKey = licenseKey;
        }
    }
}
