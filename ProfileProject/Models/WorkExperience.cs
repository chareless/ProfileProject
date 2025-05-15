using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileProject.Models
{
   
    public class WorkExperience
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Şirket Adı girmek zorunludur.")]
        [StringLength(250, ErrorMessage = "Şirket Adı 250 karakterden uzun olamaz!")]
        [Display(Name = "Şirket Adı")]
        public string CompanyName { get; set; } = "";

        [Required(ErrorMessage = "Pozisyon girmek zorunludur.")]
        [StringLength(100, ErrorMessage = "Pozisyon 100 karakterden uzun olamaz!")]
        [Display(Name = "Pozisyon")]
        public string Title { get; set; } = "";

        [Display(Name = "Açıklama")]
        public string? Information { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi girmek zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Başlangıç Tarihi")]
        public DateOnly StartWhen { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Bitiş Tarihi")]
        public DateOnly? EndWhen { get; set; }

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
        public virtual User? User { get; set; }

        public WorkExperience()
        {
            IsDeleted = false;
        }

        public WorkExperience(string companyName, string title, string info, bool ısDeleted, DateOnly start, DateOnly end, DateTime create, DateTime update,int userId, User user)
        {
            Title = title;
            CompanyName = companyName;
            Information = info;
            StartWhen = start;
            EndWhen = end;
            CreateWhen = create;
            UpdateWhen = update;
            IsDeleted = ısDeleted;
            UserId = userId;
            User = user;
        }
    }
}
