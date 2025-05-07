using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Controllers
{
    [ServiceFilter(typeof(AuthAdminFilter))]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IGeneralService generalService;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context, IGeneralService generalService)
        {
            _logger = logger;
            _context = context;
            this.generalService = generalService;
        }

        public JsonResult? GetLoginUserList()
        {
            var data = _context.LoginControls.Include(a => a.User).GroupBy(a => a.UserId).Select(g => g.First().User.Username).ToList();
            return Json(data);
        }

        public JsonResult? GetAllAccessUserList()
        {
            var userList = _context.Users.ToList();

            var data = _context.UserAccessLogs.Where(a => !a.ActionName.Contains("Layout"))
            .ToList()
            .OrderByDescending(x => x.AccessTime)
            .Select(x => userList.FirstOrDefault(u => u.Id == x.UserID)?.Username ?? "Anonim")
            .Distinct()
            .ToList();

            return Json(data);
        }

        public JsonResult? GetAllUserList()
        {
            var userList = _context.Users.ToList().Select(q => q.Username).ToList();

            return Json(userList);
        }

        public IActionResult Index(string? LoginUser, string? AccessUser, string? VisitedUser)
        {
            var userList = _context.Users.ToList();

            var model = new AccessLogDashboardViewModel
            {
                UserAccesses = _context.UserAccessLogs.Where(a => !a.ActionName.Contains("Layout"))
                 .ToList()
                 .GroupBy(x => x.UserID)
                 .Select(g => new UserAccessCount
                 {
                     UserID = g.Key,
                     Count = g.Count(),
                     UserName = userList.FirstOrDefault(u => u.Id == g.Key)?.Username ?? "Anonim"
                 })
                 .OrderByDescending(x => x.Count)
                .ToList(),

                PageAccesses = _context.UserAccessLogs.Where(a => !a.ActionName.Contains("Layout"))
                 .GroupBy(x => new { x.ControllerName, x.ActionName })
                 .Select(g => new PageAccessCount
                 {
                     Controller = g.Key.ControllerName,
                     Action = g.Key.ActionName,
                     Count = g.Count()
                 })
                 .OrderByDescending(x => x.Count)
                .ToList(),

                DailyAccesses = _context.UserAccessLogs.Where(a => !a.ActionName.Contains("Layout"))
                 .GroupBy(x => x.AccessTime.Date)
                 .Select(g => new DailyAccessCount
                 {
                     Date = g.Key,
                     Count = g.Count()
                 })
                 .OrderByDescending(x => x.Date)
                .ToList(),

                LastAccessLogs = _context.UserAccessLogs.Where(a => !a.ActionName.Contains("Layout"))
                 .ToList().Select(q => new LastAccessLogs
                 {
                     AccessTime = q.AccessTime,
                     ActionName = q.ActionName,
                     IPAddress = q.IPAddress,
                     ControllerName = q.ControllerName,
                     Description = q.Description,
                     UserID = q.UserID,
                     UserName = userList.FirstOrDefault(a => a.Id == q.UserID)?.Username ?? "Anonim"
                 }).Where(a => !string.IsNullOrEmpty(AccessUser) ? a.UserName == AccessUser : true).OrderByDescending(x => x.AccessTime).ToList(),

                LoginControls = _context.LoginControls.Include(q => q.User).Where(a => !string.IsNullOrEmpty(LoginUser) ? a.User.Username == LoginUser : true).ToList(),

                UserVisitCounts = _context.UserVisits.Include(a => a.User).Where(a => !string.IsNullOrEmpty(VisitedUser) ? a.User.Username == VisitedUser : true)
                    .GroupBy(u => new { u.UserId, u.User.Username })
                    .Select(g => new UserVisitCount
                    {
                        UserID = g.Key.UserId,
                        Username = g.Key.Username,
                        VisitedCount = g.Count()
                    }).OrderByDescending(g => g.VisitedCount)
                    .ToList(),

                UserVisitList = _context.UserAccessLogs.Where(a => a.ControllerName == "Profile" && a.ActionName == "User").ToList().Select(q => new UserVisitList
                {
                    AccessTime = q.AccessTime,
                    UserID = q.UserID,
                    VisitedID = q.VisitedUserID,
                    Username = userList.FirstOrDefault(a => a.Id == q.UserID)?.Username ?? "Anonim",
                    VisitedUsername = userList.FirstOrDefault(a => a.Id == q.VisitedUserID)?.Username ?? "Anonim"
                }).ToList()
            };

            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
