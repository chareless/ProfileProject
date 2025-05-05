using System.ComponentModel.DataAnnotations;

namespace ProfileProject.Models
{
    public class UserTheme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kullanıcı ID")]
        public int UserId { get; set; }

        [StringLength(16)]
        [Display(Name = "ThemeContrast")]
        public string? ThemeContrast { get; set; } = "false";

        [StringLength(16)]
        [Display(Name = "CaptionShow")]
        public string? CaptionShow { get; set; } = "true";

        [StringLength(16)]
        [Display(Name = "DarkLayout")]
        public string? DarkLayout { get; set; } = "false";

        [StringLength(16)]
        [Display(Name = "RtlLayout")]
        public string? RtlLayout { get; set; } = "false";

        [StringLength(16)]
        [Display(Name = "BoxContainer")]
        public string? BoxContainer { get; set; } = "false";

        [StringLength(16)]
        [Display(Name = "PresetTheme")]
        public string? PresetTheme { get; set; } = "preset-1";

        [StringLength(16)]
        [Display(Name = "Layout")]
        public string? Layout { get; set; } = "vertical";

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreateWhen { get; set; }

        [Display(Name = "Güncellenme Tarihi")]
        public DateTime UpdateWhen { get; set; }

        public UserTheme()
        {
            ThemeContrast = "false";
            CaptionShow = "true";
            DarkLayout = "false";
            RtlLayout = "false";
            BoxContainer = "false";
            PresetTheme = "preset-1";
            Layout = "vertical";
        }

        public UserTheme(int UserID, DateTime createWhen, DateTime updateWhen)
        {
            UserId = UserID;
            CreateWhen = createWhen;
            UpdateWhen = updateWhen;
        }
    }
}
