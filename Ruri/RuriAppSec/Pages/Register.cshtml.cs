using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RuriAppSec.Model;
using RuriAppSec.Pages.Services;
using RuriAppSec.ViewModels;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;

namespace RuriAppSec.Pages
{
    public class RegisterModel : PageModel
    {

        private IWebHostEnvironment _environment;

        private readonly PasswordServiceManager PasswordService;

        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        private readonly RoleManager<IdentityRole> roleManager;

        private readonly AuditLogTrailsService _auditTrailService;


        [BindProperty]
        public Register RegisterModelObject { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, AuditLogTrailsService audit, IWebHostEnvironment environment, PasswordServiceManager service)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this._environment = environment;
            this.PasswordService = service;
            _auditTrailService = audit;
        }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            Debug.WriteLine("hi");
            var isThereError = 1;
            // Check if server-side validation is valid

            if(ModelState.IsValid)
            {
                Debug.WriteLine("hisdsdf");

                //Create the Admin role if NOT exist
                if (!await roleManager.RoleExistsAsync("AgencyUser"))
                {
                    await roleManager.CreateAsync(new IdentityRole("AgencyUser"));
                }


                //checking duplicate user
                if (await userManager.FindByNameAsync(RegisterModelObject.Email) != null)
                {
                    isThereError = 0;
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Another user is registered with the same email!";
                }
                
                // Check for Server-side validation hiusing Regular Expression

                var RegularExpressionforEmail = new Regex(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");
                var RegularExpressionforPassword = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])(?!.*\\s).{12,}$");

                if (RegularExpressionforEmail.IsMatch(RegisterModelObject.Email) && RegularExpressionforPassword.IsMatch(RegisterModelObject.Password))
                {
                    isThereError = 0;
                }

                



                if (isThereError == 1)
                {
                    var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                    var data_protector = dataProtectionProvider.CreateProtector("GenerateSecretKey");

                    //sanitize whoami, firstname, lastname
                    string sanitizedwhoami = HttpUtility.HtmlEncode(RegisterModelObject.Whoami);
                    string sanitizedfirstName = HttpUtility.HtmlEncode(RegisterModelObject.First_Name);
                    string sanitizedlastName = HttpUtility.HtmlEncode(RegisterModelObject.Last_Name);

                    // Check for resume doc validation
                    if (RegisterModelObject.Upload == null)
                    {
                        return Page();
                    }
                        // if file size exceed return error
                        if (RegisterModelObject.Upload.Length > 2 * 1024 * 1024)
                        {
                        TempData["FlashMessage.Type"] = "danger";
                        TempData["FlashMessage.Text"] = "Your file upload has exceeded. Please upload another file.";
                        return Page();
                        }

                    // Read the first 4 bytes of the file
                    using (var stream = new MemoryStream())
                    {
                        await RegisterModelObject.Upload.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        var magicNumber = new byte[4];
                        stream.Read(magicNumber, 0, 4);

                        // The magic number for a .docx file is "D0CF11E0"
                        if (magicNumber[0] == 208 && magicNumber[1] == 207 && magicNumber[2] == 17 && magicNumber[3] == 224)
                        {
                            Debug.WriteLine(".docx file detected");
                        }
                        // The magic number for a .pdf file is "%PDF"
                        else if (magicNumber[0] == 37 && magicNumber[1] == 80 && magicNumber[2] == 68 && magicNumber[3] == 70)
                        {
                            Debug.WriteLine(".pdf file detected");
                        }
                        else
                        {
                            Debug.WriteLine("unknown file type");
                            TempData["FlashMessage.Type"] = "danger";
                            TempData["FlashMessage.Text"] = "File type is invalid! Please Pdf or docx only";
                            return Page();
                        }
                    }
                    var resumeFolder = "ResumeUploads";
                        // Generate Unique filename
                        var resumeFileName = Guid.NewGuid() + Path.GetExtension(RegisterModelObject.Upload.FileName);

                        // get the document path
                        var docPath = Path.Combine(_environment.ContentRootPath, "wwwroot", resumeFolder, resumeFileName);

                        using var DocumentStream = new FileStream(docPath, FileMode.Create);
                        await RegisterModelObject.Upload.CopyToAsync(DocumentStream);
                        // create user
                        var user = new ApplicationUser()
                        {
                            UserName = RegisterModelObject.Email,
                            FirstName = sanitizedfirstName,
                            LastName = sanitizedlastName,
                            NRIC = data_protector.Protect(RegisterModelObject.NRIC),
                            whoami = sanitizedwhoami,
                            DateOfBirth = RegisterModelObject.DateOfBirth,
                            Gender = RegisterModelObject.Gender,
                            TwoFactorEnabled = true
                        };

                        user.ResumeDoc = string.Format("/{0}/{1}", resumeFolder, resumeFileName);
                        // add password to password history






                    

                    Debug.WriteLine("hi3");
                    var result = await userManager.CreateAsync(user, RegisterModelObject.Password);
                    await PasswordService.Add_Password(user.Id, user.PasswordHash);
                    if (result.Succeeded)
                    {
                       //testing

                        //show mr phua
                        result = await userManager.AddToRoleAsync(user, "AgencyUser");
                        TempData["FlashMessage.Type"] = "success";
                        TempData["FlashMessage.Text"] = "Account Created";
                        

                    }

                }


            }
         


            return Page();
        }

    }
}



