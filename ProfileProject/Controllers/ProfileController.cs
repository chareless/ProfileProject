using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfileProject.Models;

namespace ProfileProject.Controllers
{
    [ServiceFilter(typeof(AuthRegisterFilter))]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _context;

        public ProfileController(ILogger<ProfileController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
