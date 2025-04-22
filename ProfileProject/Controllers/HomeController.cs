using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileProject.Models;

namespace ProfileProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        
        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search(string param)
        {
            var userData = _context.Users
                .Include(q => q.Educations)
                .Where(a =>
                    a.Email.Contains(param) ||
                    a.Username.Contains(param) ||
                    a.NameSurname.Contains(param) ||
                    a.Educations.Any(b => b.Name.Contains(param)) ||
                    a.Educations.Any(b => b.Title.Contains(param)) ||
                    a.Educations.Any(b => b.Information != null ? b.Information.Contains(param) : true) ||
                    a.Job != null ? a.Job.Contains(param) : true
    )
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
