using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RuriAppSec.Model;

namespace RuriAppSec.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly IHttpContextAccessor contxt;
        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contxt)
        {
            this.signInManager = signInManager;
            this.contxt = contxt;
        }
        public async Task<IActionResult> OnGet()
        {
            contxt.HttpContext.Session.Clear();
            await signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }
    }
}
