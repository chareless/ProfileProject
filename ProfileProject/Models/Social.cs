using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProfileProject.Models
{
   
    public class Social
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim girmek zorunludur.")]
        [Display(Name = "İsim")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Link girmek zorunludur.")]
        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "Sıralama")]
        public int Order { get; set; } = 0;

        [Display(Name = "Silindi")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreateWhen { get; set; }

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime UpdateWhen { get; set; }

        [Required]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User? User { get; set; }

        public Social()
        {
            IsDeleted = false;
        }

        public Social(string name, string link, bool ısDeleted,  DateTime create, DateTime update,int userId, User user)
        {
            Name = name;
            Link = link;
            CreateWhen = create;
            UpdateWhen = update;
            IsDeleted = ısDeleted;
            UserId = userId;
            User = user;
        }
    }
}
