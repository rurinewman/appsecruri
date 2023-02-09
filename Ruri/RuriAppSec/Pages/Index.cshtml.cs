using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RuriAppSec.Model;
using System.Web;

namespace RuriAppSec.Pages
{
    public class IndexModel : PageModel
    {


        private readonly ILogger<IndexModel> _logger;
        private UserManager<ApplicationUser> userManager { get; }

        public String newwhoami { get; set; }
        public String NRIC { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _logger = logger;
            this.userManager = userManager;

        }

        public async Task OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                //decode whoami
                newwhoami = HttpUtility.HtmlDecode(user.whoami);

                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var data_protector = dataProtectionProvider.CreateProtector("GenerateSecretKey");

                NRIC = data_protector.Unprotect(user.NRIC);
            }



        }
    }
}