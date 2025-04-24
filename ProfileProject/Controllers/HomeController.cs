using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileProject.Models;

namespace ProfileProject.Controllers
{
    //[ServiceFilter(typeof(AuthRegisterFilter))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
