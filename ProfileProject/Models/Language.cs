﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileProject.Models
{
   
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Dil girmek zorunludur.")]
        [Display(Name = "Dil")]
        public string Name { get; set; } = "";

        [Display(Name = "Açıklama")]
        public string? Information { get; set; }

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

        public Language()
        {
            IsDeleted = false;
        }

        public Language(string name, string info, bool ısDeleted,  DateTime create, DateTime update,int userId, User user)
        {
            Name = name;
            Information = info;
            CreateWhen = create;
            UpdateWhen = update;
            IsDeleted = ısDeleted;
            UserId = userId;
            User = user;
        }
    }
}
