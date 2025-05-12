using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Controllers
{
    [ServiceFilter(typeof(AuthRegisterFilter))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IGeneralService generalService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,IGeneralService generalService)
        {
            _logger = logger;
            _context = context;
            this.generalService = generalService;
        }

        public IActionResult Index()
        {
            HomeModels homeModels = new HomeModels();

            // 1. Ziyaret gruplarý: UserId + VisitCount
            var groupedVisits = _context.UserVisits
                .GroupBy(q => q.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    VisitCount = g.Count()
                })
                .ToList();

            // 2. En çok ziyaret edilen ilk 10
            var topVisited = groupedVisits
                .OrderByDescending(g => g.VisitCount)
                .Take(10)
                .ToList();

            // 3. En az ziyaret edilen ilk 10
            var leastVisited = groupedVisits
                .OrderBy(g => g.VisitCount)
                .Take(10)
                .ToList();

            // 4. Kullanýcý verilerini tek seferde al
            var userIds = topVisited.Select(x => x.UserId)
                .Concat(leastVisited.Select(x => x.UserId))
                .Distinct()
                .ToList();

            var userList = _context.Users
                .Where(u => userIds.Contains(u.Id) && !u.IsDeleted && u.IsActive)
                .ToList();

            // 5. Sýralý eþleþtirme: User + VisitCount
            homeModels.mostVisitedUsers = topVisited
                .Join(userList,
                    visit => visit.UserId,
                    user => user.Id,
                    (visit, user) => new { User = user, VisitCount = visit.VisitCount })
                .OrderByDescending(x => x.VisitCount)
                .Select(x => x.User)
                .ToList();

            homeModels.leastVisitedUsers = leastVisited
                .Join(userList,
                    visit => visit.UserId,
                    user => user.Id,
                    (visit, user) => new { User = user, VisitCount = visit.VisitCount })
                .OrderBy(x => x.VisitCount)
                .Select(x => x.User)
                .ToList();

            // Rastgele 10 kullanýcý seç
            var randomlyUsers = _context.Users.Where(a=>!a.IsDeleted && a.IsActive)
                .OrderBy(u => Guid.NewGuid())
                .Take(10)
                .ToList();

            homeModels.randomlyUsers = randomlyUsers;

            return View(homeModels);
        }

        public IActionResult Search(string? param)
        {
            if (param == null)
            {
                return View();
            }

            string normalizedParam = param.Trim().ToLower();
            string normalizedPhoneParam = normalizedParam.Replace(" ", "");

            var userData = _context.Users
                .Include(q => q.Educations)
                .Include(q => q.WorkExperiences)
                .Include(q => q.Projects)
                .AsEnumerable()
                .Where(a=>!a.IsDeleted && a.IsActive)
                .Where(a =>
                    (a.Email != null && a.Email.Trim().ToLower().Equals(normalizedParam)) ||
                    (a.Username != null && a.Username.ToLower().Contains(normalizedParam)) ||
                    (a.NameSurname != null && a.NameSurname.ToLower().Contains(normalizedParam)) ||
                    (a.Job != null && a.Job.ToLower().Contains(normalizedParam)) ||
                    (a.MobilePhone1 != null && a.MobilePhone1.Replace(" ", "").ToLower().Contains(normalizedPhoneParam)) ||
                    (a.MobilePhone2 != null && a.MobilePhone2.Replace(" ", "").ToLower().Contains(normalizedPhoneParam)) ||
                    a.Educations.Any(b =>
                        (b.Name != null && b.Name.ToLower().Contains(normalizedParam)) ||
                        (b.Title != null && b.Title.ToLower().Contains(normalizedParam))
                    ) ||
                    a.WorkExperiences.Any(b => b.CompanyName != null && b.CompanyName.ToLower().Contains(normalizedParam)) ||
                    a.Projects.Any(b => b.Name != null && b.Name.ToLower().Contains(normalizedParam))
                )
                .Select(user => new
                {
                    User = user,
                    Score =
                        (user.Email != null && user.Email.Trim().ToLower().Equals(normalizedParam) ? 150 : 0) + 
                        (user.Username != null && user.Username.ToLower().Contains(normalizedParam) ? 100 : 0) +
                        (user.NameSurname != null && user.NameSurname.ToLower().Contains(normalizedParam) ? 100 : 0) +
                        (user.Educations.Any(b => b.Name != null && b.Name.ToLower().Contains(normalizedParam)) ? 90 : 0) +
                        (user.Educations.Any(b => b.Title != null && b.Title.ToLower().Contains(normalizedParam)) ? 80 : 0) +
                        (user.Job != null && user.Job.ToLower().Contains(normalizedParam) ? 70 : 0) +
                        (user.WorkExperiences.Any(b => b.CompanyName != null && b.CompanyName.ToLower().Contains(normalizedParam)) ? 60 : 0) +
                        (user.Projects.Any(b => b.Name != null && b.Name.ToLower().Contains(normalizedParam)) ? 50 : 0) +
                        (user.MobilePhone1 != null && user.MobilePhone1.Replace(" ", "").ToLower().Contains(normalizedPhoneParam) ? 40 : 0) +
                        (user.MobilePhone2 != null && user.MobilePhone2.Replace(" ", "").ToLower().Contains(normalizedPhoneParam) ? 40 : 0)
                })
                .OrderByDescending(x => x.Score)
                .Select(x => x.User)
                .ToList();

            ViewBag.Text = param;
            return View(userData);
        }

        [HttpPost]
        public IActionResult SaveLayout([FromBody] LayoutModel model)
        {
            var userID = HttpContext.Session.GetInt32("UserId");

            if (userID != null)
            {
                var date = generalService.GetCurrentDate();
                var theme = _context.UserThemes.FirstOrDefault(a => a.UserId == userID);
                if (theme != null)
                {
                    theme.UpdateWhen = date;
                    var propertyInfo = theme.GetType().GetProperty(model.Type);
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(theme, model.Layout);
                        _context.Update(theme);
                    }
                }
                else
                {
                    theme = new UserTheme(userID.Value, date, date);
                    _context.UserThemes.Add(theme);
                    _context.SaveChanges();

                    theme = _context.UserThemes.FirstOrDefault(a => a.UserId == userID);
                    if (theme != null)
                    {
                        theme.UpdateWhen = date;
                        var propertyInfo = theme.GetType().GetProperty(model.Type);
                        if (propertyInfo != null && propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(theme, model.Layout);
                            _context.Update(theme);
                        }
                    }
                }
                _context.SaveChanges();
            }
            // Save the selected layout to the session
            HttpContext.Session.SetString(model.Type, model.Layout);
            // Return a response (optional)
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ResetLayout()
        {
            var userID = HttpContext.Session.GetInt32("UserId");

            if (userID != null)
            {
                var date = generalService.GetCurrentDate();
                var theme = _context.UserThemes.FirstOrDefault(a => a.UserId == userID);
                if (theme != null)
                {
                    _context.UserThemes.Remove(theme);
                    _context.SaveChanges();
                    theme = new UserTheme(userID.Value, date, date);
                    _context.UserThemes.Add(theme);
                    _context.SaveChanges();

                    if (theme.DarkLayout != null)
                        HttpContext.Session.SetString("DarkLayout", theme.DarkLayout);
                    if (theme.ThemeContrast != null)
                        HttpContext.Session.SetString("ThemeContrast", theme.ThemeContrast);
                    if (theme.CaptionShow != null)
                        HttpContext.Session.SetString("CaptionShow", theme.CaptionShow);
                    if (theme.PresetTheme != null)
                        HttpContext.Session.SetString("PresetTheme", theme.PresetTheme);
                    if (theme.RtlLayout != null)
                        HttpContext.Session.SetString("RtlLayout", theme.RtlLayout);
                    if (theme.BoxContainer != null)
                        HttpContext.Session.SetString("BoxContainer", theme.BoxContainer);
                    if (theme.Layout != null)
                        HttpContext.Session.SetString("Layout", theme.Layout);
                }
            }
            // Return a response (optional)
            return Json(new { success = true });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
