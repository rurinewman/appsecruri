using Microsoft.AspNetCore.Identity;

namespace RuriAppSec.Model
{
    // inheriting from identityUser
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string NRIC { get; set; }

        public string DateOfBirth { get; set; }

        public string ResumeDoc { get; set; }
        public string whoami { get; set; }



    }
}
