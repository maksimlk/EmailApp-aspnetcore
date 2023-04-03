using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmailApp.Data.Models
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Key]
        public int ID { get; set; }
        [StringLength(50)]
        public string Sender { get; set; }
        [Required(ErrorMessage = "Recipient name is not valid")]
        [ForeignKey(nameof(MailUser.UserID))]
        public MailUser Recipient { get; set; }
        [Required(ErrorMessage = "The subject is required")]
        [StringLength(255)]
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
