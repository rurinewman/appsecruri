
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace RuriAppSec.Pages.Services
{
    public class EmailService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }

        public  async Task SendForgetPassword(string email,string token)
        {
            MailMessage CraftNewMessages = new MailMessage();
            CraftNewMessages.From = new MailAddress("appsecruri@gmail.com");
            CraftNewMessages.Subject = "Password Reset";
            CraftNewMessages.To.Add(new MailAddress(email));
            CraftNewMessages.Body = "HI,Please Click the below link to reset your password." +
                 $"<a href=`https://localhost:7175/PasswordReset/ConfirmPasswordReset/?tokenid={token}&email={email}`>Confirm Email</a>";
            CraftNewMessages.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("appsecruri@gmail.com", "isecqyhndxsoltwl"),
                EnableSsl = true,
            };
            smtpClient.Send(CraftNewMessages);
                
        }

        public bool Send2FAOTP(string email, string token)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("appsecruri@gmail.com");
            mailMessage.To.Add(new MailAddress(email));

            mailMessage.Subject = "Two Factor OTP Code";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Here is the 2FA code, Please input in the field to access the website " + token;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("appsecruri@gmail.com", "isecqyhndxsoltwl"),
                EnableSsl = true,
            };

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }






    }
}