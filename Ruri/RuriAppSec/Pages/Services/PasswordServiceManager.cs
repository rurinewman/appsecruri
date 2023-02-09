using Microsoft.AspNetCore.Identity;
using RuriAppSec.Model;
using System.Diagnostics;

namespace RuriAppSec.Pages.Services
{
    public class PasswordServiceManager
    {
        private readonly AuthDbContext context;




        public PasswordServiceManager(AuthDbContext db)
        {
            context = db;
        }


        // get last updated date
        public DateTimeOffset get_latest_updated_date(string userid)
        {

            var last_updated = context.PasswordDB.Where(p => p.user_id == userid).OrderByDescending(p => p.LastUpdated).Select(p => p.LastUpdated).FirstOrDefault();
            return last_updated;
        }


        public async Task Add_Password(string user_id, string password)
        {
            // assign password to password history db
            var passwordObj = new Password() { user_id = user_id, new_Password = password };
            context.PasswordDB.Add(passwordObj);
            await context.SaveChangesAsync();
        }




        public Boolean Check_If_password_exists_in_db(ApplicationUser user, string hashed_password, string userid)
        {

            var get_list_of_history = context.PasswordDB.Where(p => p.user_id == userid).OrderByDescending(p => p.LastUpdated).Take(2).ToList();
            // retrieve the last 2 password saved in db
            var password_hash_method = new PasswordHasher<ApplicationUser>();

            var password_exists = false;

            foreach (var i in get_list_of_history)
            {
                // check if the password is equivalent to the hashed passwords in db
                if (password_hash_method.VerifyHashedPassword(user, i.new_Password, hashed_password) == PasswordVerificationResult.Success)
                {
                    password_exists = true;
                }
            }
            
            if(password_exists)
            {
                return true;
            }
            return false;




        }
        // Check if the password change is within the minimum password age
        public bool check_password_minimum_time(DateTimeOffset last_updated)
        {
            var check_current_time = DateTimeOffset.Now - last_updated;
            
            // users cannot change password if its within one minute of last change
            if (check_current_time < TimeSpan.FromMinutes(1))
            {
                return false;
            }
            return true;
        }



        // check if password expire, if expire -> return true meaning users have to change
        public bool check_if_password_expiry(DateTimeOffset last_updated)
        {
            var get_date = DateTime.Now - last_updated;
            // if the last changed date has exceed 30 days -> return true, user will have to change their password
            if (get_date.TotalDays > 30)
            {
                Debug.WriteLine("Password has expire!");
                return true;
            }
            return false;
        }









    }
}
