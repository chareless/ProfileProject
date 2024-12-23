using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfileProject.Models;
using ProfileProject.Services.LoginServices;
using static ProfileProject.Models.LoginModels;

namespace ProfileProject.Controllers
{
    [ServiceFilter(typeof(AuthRegisterFilter))]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILoginService loginService;

        public LoginController(ILogger<LoginController> logger,ApplicationDbContext context, ILoginService loginService)
        {
            _logger = logger;
            _context = context;
            this.loginService = loginService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (!loginService.LoginControl(model))
                {
                    ModelState.AddModelError("", "Kullanýcý bilgileri doðrulanamadý.");
                    return View(model);
                }

                var user = loginService.GetUserData(model);

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("SessionStartTime", DateTime.Now.ToString());
                HttpContext.Session.SetInt32("IsAdmin", user.IsAdmin ? 1 : 0);
                HttpContext.Session.SetInt32("IsActive", user.IsActive ? 1 : 0);
                HttpContext.Session.SetInt32("IsDeleted", user.IsDeleted ? 1 : 0);
                return RedirectToAction("Index", "Home"); 
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("SessionStartTime");
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("IsActive");
            HttpContext.Session.Remove("IsDeleted");
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
