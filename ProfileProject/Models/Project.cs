﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileProject.Models
{
   
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Proje Adı girmek zorunludur.")]
        [StringLength(250, ErrorMessage = "Proje Adı 250 karakterden uzun olamaz!")]
        [Display(Name = "Proje Adı")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public string Information { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartWhen { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndWhen { get; set; }

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
        public virtual User User { get; set; }

        public Project()
        {
            IsDeleted = false;
        }

        public Project(string name,string info, bool ısDeleted, DateTime start, DateTime end, DateTime create, DateTime update,int userId, User user)
        {
            Name = name;
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
