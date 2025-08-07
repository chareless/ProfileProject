using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProfileProject.Models
{

    public class Reference
    {
        [Key]
        public int Id { get; set; }

        [StringLength(250, ErrorMessage = "Şirket Adı 250 karakterden uzun olamaz!")]
        [Display(Name = "Şirket Adı")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "İsim Soyisim girmek zorunludur.")]
        [StringLength(100, ErrorMessage = "İsim Soyisim 100 karakterden uzun olamaz!")]
        [Display(Name = "İsim Soyisim")]
        public string Name { get; set; } = "";

        [StringLength(100, ErrorMessage = "Pozisyon 100 karakterden uzun olamaz!")]
        [Display(Name = "Pozisyon")]
        public string? Title { get; set; }

        [Display(Name = "Açıklama")]
        public string? Information { get; set; }

        [EmailAddress(ErrorMessage = "Geçersiz mail formatı!")]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; }

        [StringLength(15, ErrorMessage = "Telefon numarası 15 karakterden uzun olamaz.")]
        [Display(Name = "Telefon")]
        public string? MobilePhone { get; set; }

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

        public Reference()
        {
            IsDeleted = false;
        }

        public Reference(string companyName, string name, string title , string info, string email, string phone, bool ısDeleted,  DateTime create, DateTime update,int userId, User user)
        {
            Title = title;
            CompanyName = companyName;
            Name = name;
            Information = info;
            Email = email;
            MobilePhone = phone;
            CreateWhen = create;
            UpdateWhen = update;
            IsDeleted = ısDeleted;
            UserId = userId;
            User = user;
        }
    }
}
