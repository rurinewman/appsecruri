using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RuriAppSec.Model;
using RuriAppSec.Pages.Services;
using RuriAppSec.ViewModels;
using System.Text;

namespace RuriAppSec.Pages
{


    public class LoginModel : PageModel
    {
        private readonly PasswordServiceManager PasswordService;


        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly AuditLogTrailsService _auditTrailService;

        private UserManager<ApplicationUser> userManager { get; }

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogTrailsService audit, PasswordServiceManager passwordService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _auditTrailService = audit;
            PasswordService = passwordService;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public Login LoginModelObject { get; set; }



        public async Task<IActionResult> OnPost()
        {
            var IsThereError = false;
            if (ModelState.IsValid)
            {
                var userAccount = await userManager.FindByNameAsync(LoginModelObject.Email);
                if (userAccount != null)
                {
                    // check if account is locked
                    if (await userManager.IsLockedOutAsync(userAccount))
                    {
                        await _auditTrailService.Track(userAccount.Id, "User Account locked after 3 Attempted login");
                        TempData["FlashMessage.Type"] = "danger";
                        TempData["FlashMessage.Text"] = "Account Locked after 3 unsuccessful attempts! Please try again after 2 minutes.";
                        return RedirectToPage("/Login");

                    }

                    // PROVES THERE IS ERROR
                    if (IsThereError)
                    {
                        TempData["FlashMessage.Type"] = "danger";
                        TempData["FlashMessage.Text"] = "Oops, Please try again, there is an error";
                        return RedirectToPage("/Login");
                    }


                    //// check if email is verified
                    //if (await userManager.IsEmailConfirmedAsync(user) == false)
                    //{
                    //    TempData["FlashMessage.Type"] = "danger";
                    //    TempData["FlashMessage.Text"] = "Oops, It looks like the account is not verified";
                    //    return RedirectToPage("/Login");
                    //}




                    // RESET KEYS 
                    await userManager.ResetAuthenticatorKeyAsync(userAccount);
                    await userManager.GetAuthenticatorKeyAsync(userAccount);

                    
                    var result = await signInManager.PasswordSignInAsync(LoginModelObject.Email, LoginModelObject.Password, true, true);

                    // if password expires, prompt user to change password
                    if(result.RequiresTwoFactor && PasswordService.check_if_password_expiry(PasswordService.get_latest_updated_date(userAccount.Id)))
                    {
                        var code = await userManager.GeneratePasswordResetTokenAsync(userAccount);
                        // encoded token
                        byte[] inputBytes = Encoding.UTF8.GetBytes(code);
                        var EncodedToken = WebEncoders.Base64UrlEncode(inputBytes);
                        return RedirectToPage("PasswordReset/ConfirmPasswordReset", new { email = LoginModelObject.Email, tokenid = EncodedToken });
                        TempData["FlashMessage.Type"] = "success";
                        TempData["FlashMessage.Text"] = "Password reset email has been sent to you.";


                    }





                    // once login, prompt to login 2 factor
                    if (result.RequiresTwoFactor)
                    {
                        TempData["FlashMessage.Type"] = "success";
                        TempData["FlashMessage.Text"] = "OTP Code has been sent to your email! Please input it to confirm";
                        return RedirectToPage("OTPVerification");
                    }
                    // IF RESULT DOES NOT SUCCEED
                    if (!result.Succeeded)
                    {
                        TempData["FlashMessage.Type"] = "danger";
                        TempData["FlashMessage.Text"] = "Oops, please ensure that the correct credentials are entered";
                        return RedirectToPage("/Login");
                    }

                }

            }
            TempData["FlashMessage.Type"] = "danger";
            TempData["FlashMessage.Text"] = "Oops, please ensure that the correct credentials are entered";
            return RedirectToPage("/Login");
        }



    }
}
