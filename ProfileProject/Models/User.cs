using System.ComponentModel.DataAnnotations;

namespace ProfileProject.Models
{
   
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim girmek zorunludur.")]
        [StringLength(50, ErrorMessage = "İsim 50 karakterden uzun olamaz!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyisim girmek zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyisim 50 karakterden uzun olamaz!")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Mail adresi girmek zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz mail formatı!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası girmek zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz!")]
        [StringLength(15, ErrorMessage = "Telefon numarası 15 karakterden uzun olamaz.")]
        public string MobilePhone { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı girmek zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı 50 karakterden uzun olamaz!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre girmek zorunludur.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Şifre uzunluğu 6 ile 20 karakter arasında olmalıdır.")] // Şifre uzunluğu
        public string Password { get; set; }

        [Required(ErrorMessage = "Doğum tarihi girmek zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public User()
        {
            IsDeleted = false;
            IsActive = true;
        }

        public User(string name, string surname, string email,string mobilePhone, string username, string password, DateTime birthday, bool ısDeleted, bool ısActive)
        {
            Name = name;
            Surname = surname;
            Email = email;
            MobilePhone = mobilePhone;
            Username = username;
            Password = password;
            Birthday = birthday;
            IsDeleted = ısDeleted;
            IsActive = ısActive;
        }
    }
}
