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

        public IActionResult Index(string? LoginUser, string? AccessUser, string? VisitedUser, string? UserForList, string? VisitedForList, string? UserForGroup, string? VisitedForGroup,
            DateTime? allAccessStartDateParam,DateTime? allAccessEndDateParam, int? allAccessDateType
            )
        {
            var userList = _context.Users.ToList();
            int? userId = 0;
            int? accessId = 0;
            int? visitedId = 0;
            int? userGroupId = 0;
            int? visitedGroupId = 0;

           

            DateTime? allAccessStartDate = allAccessStartDateParam;
            DateTime? allAccessEndDate = allAccessEndDateParam;

            if(allAccessDateType == null || (allAccessDateType != null && allAccessDateType == 0))
            {
                //BAÞLANGIÇ BÝTÝÞ TARÝHÝ GÜN SONU VE GÜN BAÞI OLARAK AYARLANACAK
                allAccessStartDate = DateTime.Now.Date;
                allAccessEndDate = DateTime.Now.Date;
            }
            else if(allAccessDateType != null && allAccessDateType == 1)
            {
                allAccessStartDate = allAccessStartDateParam;
                allAccessEndDate = allAccessEndDateParam;
            }
            else if(allAccessDateType != null && allAccessDateType == 2)
            {
                allAccessStartDate = null;
                allAccessEndDate = null;
            }

           ViewData["AllAccessStartDate"] = allAccessStartDate;
            ViewData["AllAccessEndDate"] = allAccessEndDate;

            if (UserForList != null)
            {
                userId = userList.FirstOrDefault(a => a.Username == UserForList)?.Id;
            }
            if (VisitedForList != null)
            {
                visitedId = userList.FirstOrDefault(a => a.Username == VisitedForList)?.Id;
            }
            if(AccessUser != null)
            {
                accessId = userList.FirstOrDefault(a => a.Username == AccessUser)?.Id;
            }
            if (UserForGroup != null)
            {
                userGroupId = userList.FirstOrDefault(a => a.Username == UserForGroup)?.Id;
            }
            if (VisitedForGroup != null)
            {
                visitedGroupId = userList.FirstOrDefault(a => a.Username == VisitedForGroup)?.Id;
            }

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
                  .Where(a => accessId != 0 ? (a.UserID == accessId) : true)
                  .Where(a=> allAccessStartDate != null ? (a.AccessTime >= allAccessStartDate) : true)
                  .Where(a=> allAccessEndDate != null ? (a.AccessTime <= allAccessEndDate): true)
                 .ToList().Select(q => new LastAccessLogs
                 {
                     AccessTime = q.AccessTime,
                     ActionName = q.ActionName,
                     IPAddress = q.IPAddress,
                     ControllerName = q.ControllerName,
                     Description = q.Description,
                     UserID = q.UserID,
                     UserName = userList.FirstOrDefault(a => a.Id == q.UserID)?.Username ?? "Anonim"
                 }).OrderByDescending(x => x.AccessTime).ToList(),

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

                UserVisitList = _context.UserAccessLogs.Where(a => a.ControllerName == "Profile" && a.ActionName == "User")
                .Where(a=> userId != 0 ? (a.UserID ==userId) : true)
                .Where(a=> visitedId != 0 ? (a.VisitedUserID == visitedId) : true)
                .ToList().Select(q => new UserVisitList
                {
                    AccessTime = q.AccessTime,
                    UserID = q.UserID,
                    VisitedID = q.VisitedUserID,
                    Username = userList.FirstOrDefault(a => a.Id == q.UserID)?.Username ?? "Anonim",
                    VisitedUsername = userList.FirstOrDefault(a => a.Id == q.VisitedUserID)?.Username ?? "Anonim"
                }).ToList(),

                UserVisitGroups = _context.UserAccessLogs
                .Where(a => a.ControllerName == "Profile" && a.ActionName == "User")
                .Where(a => userGroupId != 0 ? (a.UserID == userGroupId) : true)
    .           Where(a => visitedGroupId != 0 ? (a.VisitedUserID == visitedGroupId) : true).ToList()
    .           GroupBy(q => new { q.UserID, q.VisitedUserID })
                .Select(g => new UserVisitGroup
                {
                    count = g.Count(),
                    UserID = g.Key.UserID,
                    VisitedID = g.Key.VisitedUserID,
                    Username = userList.FirstOrDefault(a => a.Id == g.Key.UserID)?.Username ?? "Anonim",
                    VisitedUsername = userList.FirstOrDefault(a => a.Id == g.Key.VisitedUserID)?.Username ?? "Anonim"
                })
    .           ToList()
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
