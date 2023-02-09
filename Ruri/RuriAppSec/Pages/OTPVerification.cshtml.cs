using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RuriAppSec.Model;
using RuriAppSec.Pages.Services;
using System.Diagnostics;

namespace RuriAppSec.Pages
{
    public class OTPVerificationModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager { get; }

        private readonly IHttpContextAccessor contxt;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private EmailService emailService;

        private readonly AuditLogTrailsService _auditTrailService;


        [BindProperty]
        public string otp_token_bind { get; set; }
        [BindProperty]

        public string OTPCode { get; set; }

        public OTPVerificationModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contxt, AuditLogTrailsService audit, EmailService service)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _auditTrailService = audit;
            this.contxt = contxt;
            this.emailService = service;
        }

        public async Task<IActionResult> OnGet()
        {
            // get user that is half login
            var getUserAccount = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            // generate 2 factor authentication token
            var otp_token = await _userManager.GenerateTwoFactorTokenAsync(getUserAccount, "Email");
            // send otp to user email
            Debug.WriteLine(getUserAccount.UserName);
            Debug.WriteLine("hi");

            emailService.Send2FAOTP(getUserAccount.UserName, otp_token);
            return Page();



        }

        public async Task<IActionResult> OnPost()
        {
            var result = await _signInManager.TwoFactorSignInAsync("Email", OTPCode, false, false);
            if(result.Succeeded)
            {
                Guid myuuid = Guid.NewGuid();

                contxt.HttpContext.Session.SetString("UniqueID", myuuid.ToString());
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = "Successfully login!!";
                var getUserAccount = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                await _auditTrailService.Track(getUserAccount.Id, "User logged in with OTP verification");

                return RedirectToPage("Index");
            }
            return Page();
        }



    }
}
