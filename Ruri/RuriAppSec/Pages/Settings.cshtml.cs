using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RuriAppSec.Model;
using RuriAppSec.ViewModels;
using System.Data;

namespace RuriAppSec.Pages
{



    [Authorize(Roles = "AgencyUser")]
    public class SettingsModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }

        [BindProperty]
        public Setting Settings_Model { get; set; } = new();



        public SettingsModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await userManager.GetUserAsync(User);
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protect_class = dataProtectionProvider.CreateProtector("GenerateSecretKey");

            if (user != null)
            {

                Settings_Model.FirstName = user.FirstName;
                Settings_Model.NRIC = protect_class.Unprotect(user.NRIC);
            }
            return Page();



        }
    }
}
