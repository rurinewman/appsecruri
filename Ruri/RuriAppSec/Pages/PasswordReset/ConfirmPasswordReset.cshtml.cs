using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using RuriAppSec.Model;
using RuriAppSec.Pages.Services;
using System.Diagnostics;
using System.Text;

namespace RuriAppSec.Pages.PasswordReset
{
    public class ConfirmPasswordResetModel : PageModel
    {

        private readonly IHttpContextAccessor contxt;

        [BindProperty]
        public string token { get; set; }

        [BindProperty]
        public string Email { get; set; }


        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string CurrentPassword { get; set; }


        [BindProperty]
        public string NewPassword { get; set; }


        [BindProperty]
        public string ConfirmNewPassword { get; set; }

        public UserManager<ApplicationUser> userManager { get; }


        private readonly PasswordServiceManager PasswordService;


        public ConfirmPasswordResetModel(UserManager<ApplicationUser> userManager, IHttpContextAccessor contxt, PasswordServiceManager service)
        {
            this.userManager = userManager;
            this.contxt = contxt;
            this.PasswordService = service;
        }


        public void OnGet(string tokenid, string email)
        {
            token = tokenid;
            Email = email;

        }


        public async Task<IActionResult> OnPost()
        {

                var user = await userManager.FindByNameAsync(Email);


                // check if current password is correct

                var result = await userManager.CheckPasswordAsync(user, CurrentPassword);
            //if (result == false)
            //{
            //    Debug.WriteLine("current password is not correct");
            //    TempData["FlashMessage.Type"] = "danger";
            //    TempData["FlashMessage.Text"] = "Your current password is incorrect.";
            //    return Page();
            //}
            //else
            //{
                // check if password already exists in db -> if exists, reject
                if (PasswordService.Check_If_password_exists_in_db(user, NewPassword, user.Id))
                {
                    Debug.WriteLine(NewPassword + "exists already cannot reuse 2 of the previous password");
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Cannot reuse previous 2 passwords";
                    return Page();
                }

                // check if password has just been changed
                if (PasswordService.check_password_minimum_time(PasswordService.get_latest_updated_date(user.Id)) == false)
                {
                    Debug.WriteLine("Please wait for 1Minute before changing!!");
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Please wait 1 minute before changing your password again";
                    return Page();
                }

                

                // CHANGE PASSWORD PROCESS BELOW
                // GETT THE DECODED TOKEN
                byte[] decodedBytes = WebEncoders.Base64UrlDecode(token);
                var getToken = Encoding.UTF8.GetString(decodedBytes);


                var resetPassResult = await userManager.ResetPasswordAsync(user, getToken, ConfirmNewPassword);
                if (!resetPassResult.Succeeded)
                {
                    Debug.WriteLine(token);
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Passwords must be at least 12 characters and contain least: 1 upper case (A-Z), 1 lower case (a-z), 1 number (0-9) and special character, with no spaces";
                    Debug.WriteLine("failed");
                    return Page();

                }
                 // save the hashed value of the new password
                  await PasswordService.Add_Password(user.Id, user.PasswordHash);


            //}
            Debug.WriteLine("password changed");
            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = "Your password has been changed successfully";
            return RedirectToPage("../Login");
           }
       }


  }
