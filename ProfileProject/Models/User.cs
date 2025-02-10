﻿using System.ComponentModel.DataAnnotations;

namespace ProfileProject.Models
{
   
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim girmek zorunludur.")]
        [StringLength(50, ErrorMessage = "İsim 50 karakterden uzun olamaz!")]
        [Display(Name = "İsim Soyisim")]
        public string NameSurname { get; set; }

        [Required(ErrorMessage = "Mail adresi girmek zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz mail formatı!")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz!")]
        [StringLength(15, ErrorMessage = "Telefon numarası 15 karakterden uzun olamaz.")]
        [Display(Name = "Telefon-1")]
        public string MobilePhone1 { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz!")]
        [StringLength(15, ErrorMessage = "Telefon numarası 15 karakterden uzun olamaz.")]
        [Display(Name = "Telefon-2")]
        public string MobilePhone2 { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı girmek zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı 50 karakterden uzun olamaz!")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre girmek zorunludur.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Şifre uzunluğu 6 ile 20 karakter arasında olmalıdır.")] // Şifre uzunluğu
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Doğum tarihi girmek zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime Birthday { get; set; }

        [Display(Name = "Hakkında")]
        public string About { get; set; }

        [Display(Name = "Medeni Durumu")]
        public string Marriage { get; set; }

        [Display(Name = "Sürücü Belgesi")]
        public string DrivingLicense { get; set; }

        [Display(Name = "Askerlik")]
        public string Military { get; set; }

        [Display(Name = "Hobiler")]
        public string Hobbies { get; set; }

        [Display(Name = "Silindi")]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; } = false;

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreateWhen { get; set; }

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime UpdateWhen { get; set; }

        [Display(Name = "Ziyaretçi Sayısı")]
        public int VisitorCount { get; set; } = 0;

        [Display(Name = "Eğitim Bilgileri")]
        public virtual List<Education> Educations { get; set; } = new List<Education>();

        [Display(Name = "İş Tecrübeleri")]
        public virtual List<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();

        [Display(Name = "Beceriler")]
        public virtual List<Skill> Skills { get; set; } = new List<Skill>();

        [Display(Name = "Referanslar")]
        public virtual List<Reference> References { get; set; } = new List<Reference>();

        [Display(Name = "Diller")]
        public virtual List<Language> Languages { get; set; } = new List<Language>();

        [Display(Name = "Projeler")]
        public virtual List<Project> Projects { get; set; } = new List<Project>();

        [Display(Name = "Sertifikalar")]
        public virtual List<Certificate> Certificates { get; set; } = new List<Certificate>();

        public User()
        {
            IsDeleted = false;
            IsActive = true;
            IsAdmin = false;
            VisitorCount = 0;
        }

        public User(string name, string email,string mobilePhone1,string mobilePhone2, string username, string password, DateTime birthday, bool ısDeleted, bool ısActive, bool ısAdmin, DateTime create, DateTime update)
        {
            NameSurname = name;
            Email = email;
            MobilePhone1 = mobilePhone1;
            MobilePhone2 = mobilePhone2;
            Username = username;
            Password = password;
            Birthday = birthday;
            IsDeleted = ısDeleted;
            IsActive = ısActive;
            IsAdmin = ısAdmin;
            CreateWhen = create;
            UpdateWhen = update;
        }
    }
}
