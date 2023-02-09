using System.ComponentModel.DataAnnotations.Schema;

namespace RuriAppSec.Model
{
    public class Password
    {

        public string new_Password { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public string user_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string PasswordID { get; set; }

        public Password()
        {
            LastUpdated = DateTimeOffset.Now;

        }


    }
}
