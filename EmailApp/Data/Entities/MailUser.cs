using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailApp.Data.Models
{
    public class MailUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

}
