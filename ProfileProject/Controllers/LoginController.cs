using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using ProfileProject.Services.LoginServices;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILoginService loginService;
        private readonly IGeneralService generalService;

        public LoginController(ILogger<LoginController> logger,ApplicationDbContext context, ILoginService loginService,IGeneralService generalService)
        {
            _logger = logger;
            _context = context;
            this.loginService = loginService;
            this.generalService = generalService;
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
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Kullanýcý",
                        Message = "Kullanýcý bilgileri doðrulanamadý!"
                    });
                    return View(model);
                }

                var user = loginService.GetUserData(model);
                var date = generalService.GetCurrentDate();
                var theme = loginService.GetUserTheme(user.Id);

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Picture", user.Picture);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("SessionStartTime", date.ToString());
                HttpContext.Session.SetInt32("IsAdmin", user.IsAdmin ? 1 : 0);

                loginService.LoginUser(user.Id);

                if (theme == null)
                {
                    var userTheme = new UserTheme(user.Id, date, date);
                    _context.UserThemes.Add(userTheme);
                    _context.SaveChanges();
                }
                else
                {
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

                return RedirectToAction("Index", "Home"); 
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext?.Session?.Remove("Username");
            HttpContext?.Session?.Remove("Picture");
            HttpContext?.Session?.Remove("Email");
            HttpContext?.Session?.Remove("UserId");
            HttpContext?.Session?.Remove("SessionStartTime");
            HttpContext?.Session?.Remove("IsAdmin");
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
