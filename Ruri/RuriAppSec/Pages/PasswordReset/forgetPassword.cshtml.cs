using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RuriAppSec.Model;
using RuriAppSec.Pages.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace RuriAppSec.Pages.PasswordReset
{
    public class forgetPasswordModel : PageModel
    {
        private readonly EmailService email;

        private UserManager<ApplicationUser> _usermanager { get; }


        [BindProperty]
        [Required]
        public string EmailAddress { get; set; }


        public forgetPasswordModel(EmailService send_email, UserManager<ApplicationUser> userManager)
        {
            this.email = send_email;
            this._usermanager = userManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _usermanager.FindByNameAsync(EmailAddress);
            if (user != null)
            {
                // if valid
            
                var code = await _usermanager.GeneratePasswordResetTokenAsync(user);
                // encoded token
                byte[] inputBytes = Encoding.UTF8.GetBytes(code);
                var EncodedToken = WebEncoders.Base64UrlEncode(inputBytes);


                await email.SendForgetPassword(EmailAddress, EncodedToken);
                Debug.WriteLine(code);

            } else
            {
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = "Error, Please ensure that email is correct";
                return RedirectToPage("Login");
            }
            return RedirectToPage("../index");



        }




    }
}
