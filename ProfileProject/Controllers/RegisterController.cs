using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;

namespace ProfileProject.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IGeneralService generalService;

        public RegisterController(ILogger<RegisterController> logger,ApplicationDbContext context,IGeneralService generalService)
        {
            _logger = logger;
            _context = context;
            this.generalService = generalService;
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
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Email == model.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu kullanýcý adý veya email adresi zaten alýnmýþ.");
                    return View(model);
                }

                var date = generalService.GetCurrentDate();

                model.Password = generalService.SetHash(model.Password);
                ConfirmPassword = generalService.SetHash(ConfirmPassword);

                if(model.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("", "Þifre doðrulanamadý.");
                    return View(model);
                }

                var user = new User(model.NameSurname, model.Email,model.MobilePhone1,model.MobilePhone2,model.Username,model.Password,model.Birthday,false,true,false,date,date);

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
