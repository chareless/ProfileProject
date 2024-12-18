using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfileProject.Models;
using ProfileProject.Services.LoginServices;

namespace ProfileProject.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILoginService loginService;

        public RegisterController(ILogger<RegisterController> logger,ApplicationDbContext context,ILoginService loginService)
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
        public async Task<IActionResult> Index(User model,string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu kullanýcý adý zaten alýnmýþ.");
                    return View(model);
                }

                model.Password = loginService.SetHash(model.Password);
                ConfirmPassword = loginService.SetHash(ConfirmPassword);

                if(model.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("", "Þifre doðrulanamadý.");
                    return View(model);
                }

                var user = new User(model.Name, model.Surname, model.Email,model.MobilePhone,model.Username,model.Password,model.Birthday,false,true);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Kayýt baþarýlýysa anasayfaya yönlendir
            }

            return View(model); // Model geçersizse ayný sayfayý tekrar göster
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
