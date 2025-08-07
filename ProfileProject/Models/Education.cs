using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProfileProject.Models
{
   
    public class Education
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Eğitim Derecesi girmek zorunludur.")]
        [StringLength(50, ErrorMessage = "Eğitim Derecesi 50 karakterden uzun olamaz!")]
        [Display(Name = "Eğitim Derecesi")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "Okul Adı girmek zorunludur.")]
        [StringLength(250, ErrorMessage = "Okul Adı 250 karakterden uzun olamaz!")]
        [Display(Name = "Okul Adı")]
        public string Name { get; set; } = "";

        [Display(Name = "Mezuniyet Notu")]
        public string? GradePoint { get; set; }

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
        [JsonIgnore]
        public virtual User? User { get; set; }

        public Education()
        {
            IsDeleted = false;
        }

        public Education(string title, string name,string point,string info, bool ısDeleted, DateOnly start, DateOnly end, DateTime create, DateTime update, int userId, User user)
        {
            Title = title;
            Name = name;
            GradePoint = point;
            Information = info;
            StartWhen = start;
            EndWhen = end;
            CreateWhen = create;
            UpdateWhen = update;
            IsDeleted = ısDeleted;
            User = user;
            UserId = userId;
        }
    }
}
