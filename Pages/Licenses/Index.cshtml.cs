using Microsoft.AspNetCore.Mvc.RazorPages;
using Youxel.Check.LicensesGenerator.Domain.Entities;
using Youxel.Check.LicensesGenerator.Infrastructure.Repositories;

namespace Youxel.Check.LicensesGenerator.Pages.Licenses
{
    public class IndexModel : PageModel
    {
        private readonly ILicensesRepository _licenseRepository;

        public IndexModel(ILicensesRepository licenseRepository)
        {
            _licenseRepository = licenseRepository;
        }

        public List<License> Licenses { get; set; }

        public void OnGet()
        {
            Licenses = _licenseRepository.GetLicenses();
        }
    }
}
