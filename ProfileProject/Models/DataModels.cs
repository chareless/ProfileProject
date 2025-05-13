using System.ComponentModel.DataAnnotations;

namespace ProfileProject.Models
{
    public class DataModels
    {
        public class LoginModel
        {
            [Required(ErrorMessage = "Kullanıcı Adı girmek zorunludur.")]
            [StringLength(50, ErrorMessage = "Kullanıcı adı 50 karakterden uzun olamaz!")]
            [Display(Name = "Kullanıcı Adı")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Şifre girmek zorunludur.")]
            [MinLength(6, ErrorMessage = "Şifre uzunluğu en az 6 karakter olmalıdır!")]
            [Display(Name = "Şifre")]
            public string Password { get; set; }
        }

        public class ChangePasswordModel
        {
            public string? Username { get; set; }

            [Required(ErrorMessage = "Eski Şifre girmek zorunludur.")]
            [Display(Name = "Eski Şifre")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Yeni Şifre girmek zorunludur.")]
            [MinLength(6, ErrorMessage = "Yeni Şifre uzunluğu en az 6 karakter olmalıdır!")]
            [Display(Name = "Yeni Şifre")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Yeni Şifre Tekrar girmek zorunludur.")]
            [Display(Name = "Yeni Şifre Tekrar")]
            public string ConfirmPassword { get; set; }
        }

        public class RegisterModel
        {
            public User User { get; set; }

            [Required(ErrorMessage = "Şifre girmek zorunludur.")]
            [Display(Name = "Şifre Tekrar")]
            public string ConfirmPassword { get; set; }
        }

        public class LayoutModel
        {
            public string Layout { get; set; }
            public string Type { get; set; }
        }

        public class HomeModels
        {
            public List<User> mostVisitedUsers { get; set; }
            public List<User> leastVisitedUsers { get; set; }
            public List<User> randomlyUsers { get; set; }
        }

        public class EducationDto
        {
            public string Title { get; set; }
            public string Name { get; set; }
            public string? GradePoint { get; set; }
            public string? Information { get; set; }
            public string StartWhen { get; set; } // ISO-8601 (yyyy-MM-dd)
            public string? EndWhen { get; set; }
        }


    }
}
