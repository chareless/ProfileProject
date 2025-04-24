using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileProject.Models
{
   
    public class UserVisit
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreateWhen { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public UserVisit()
        {

        }

        public UserVisit(int userId)
        {
            CreateWhen = DateTime.Now;
            UserId = userId;
        }
    }
}
