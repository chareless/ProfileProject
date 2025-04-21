using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileProject.Models
{
    public class LoginControl
    {
        [Key]
        public int ID { get; set; }

        [DisplayName("Giriş Tarihi")]
        [DataType(DataType.Date)]
        public DateTime LoginDate { get; set; }

        [Required]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public LoginControl()
        {
            LoginDate = DateTime.Now;
        }
        public LoginControl(DateTime time, int user)
        {
            LoginDate = time;
            UserId = user;
        }
    }
}
