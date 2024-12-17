using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProfileProject.Models;

namespace ProfileProject.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterController(ILogger<RegisterController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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
                    ModelState.AddModelError("", "Bu kullan�c� ad� zaten al�nm��.");
                    return View(model);
                }

                if(model.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("", "�ifre do�rulanamad�.");
                    return View(model);
                }

                var user = new User(model.Name, model.Surname, model.Email,model.MobilePhone,model.Username,model.Password,model.Birthday,false,true);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home"); // Kay�t ba�ar�l�ysa anasayfaya y�nlendir
            }

            return View(model); // Model ge�ersizse ayn� sayfay� tekrar g�ster
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
