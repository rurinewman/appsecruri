using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RuriAppSec.ViewModels
{
    public class Register
    {

        [Required]
        
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessage ="Your email is invalid")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-])(?!.*\\s).{12,}$", ErrorMessage = "Passwords must be at least 12 characters and contain least: 1 upper case (A-Z), 1 lower case (a-z), 1 number (0-9) and special character, with no spaces")]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }


        [Required]
        public string Gender { get; set; }


        [Required]
        [RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "Please Re-entry NRIC not valid"), MaxLength(9)]
        public string NRIC { get; set; }


        [Required]
        [Display(Name = "Date of birth")]
        public string DateOfBirth { get; set; }


        [Required]
        [Display(Name = "Who am I")]
        public string Whoami { get; set; }


        public string? ResumeDoc { get; set; }

        [BindProperty]
        public IFormFile? Upload { get; set; }


        [Required]
        [Display(Name = "First Name")]
        [RegularExpression("^[^-\\s][a-zA-Z0-9_\\s-]+$", ErrorMessage = "Please only enter alphabets")]
        public string First_Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }



    }
}
