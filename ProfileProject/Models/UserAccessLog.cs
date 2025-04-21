using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileProject.Models
{
    public class UserAccessLog
    {
        [Key]
        public int ID { get; set; }

        [DisplayName("Kullanıcı")]
        public int? UserID { get; set; }

        [DisplayName("Controller")]
        public string ControllerName { get; set; } = "";

        [DisplayName("Action")]
        public string ActionName { get; set; } = "";

        [DisplayName("Giriş Tarihi")]
        public DateTime AccessTime { get; set; } = DateTime.Now;

        [DisplayName("IP Adresi")]
        public string IPAddress { get; set; } = "";

        [DisplayName("URL")]
        public string? Description { get; set; } = "";

        [DisplayName("Ziyaret Edilen Kullanıcı")]
        public int? VisitedUserID { get; set; }

        public UserAccessLog() 
        {
            AccessTime = DateTime.Now;
        }
        public UserAccessLog(int? userID, string controller, string action, DateTime time, string ip, string desc, int? visited)
        {
            UserID = userID;
            ControllerName = controller;
            ActionName = action;
            AccessTime = time;
            IPAddress = ip;
            Description = desc;
            VisitedUserID = visited;
        }
    }
}
