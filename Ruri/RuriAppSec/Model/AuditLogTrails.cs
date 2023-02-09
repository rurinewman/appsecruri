using System.ComponentModel.DataAnnotations.Schema;

namespace RuriAppSec.Model
{
    public class AuditLogTrails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Details { get; set; }


        public string userID { get; set; }

        public DateTime Date { get; set; }


    }
}
