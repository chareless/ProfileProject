using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using static ProfileProject.Models.DataModels;

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
        public async Task<IActionResult> Index(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var modelUser = model.User;
                var existingUsername = _context.Users.FirstOrDefault(u => u.Username == modelUser.Username);
                var existingMail= _context.Users.FirstOrDefault(u => u.Email == modelUser.Email);

                if (existingUsername != null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Kullanýcý",
                        Message = "Bu kullanýcý adý zaten alýnmýþ!"
                    });
                    return View(model);
                }

                if (existingMail != null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Kullanýcý",
                        Message = "Bu email adresi zaten alýnmýþ!"
                    });
                    return View(model);
                }

                var date = generalService.GetCurrentDate();

                modelUser.Password = generalService.SetHash(modelUser.Password);
                model.ConfirmPassword = generalService.SetHash(model.ConfirmPassword);

                if(modelUser.Password != model.ConfirmPassword)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Þifre Doðrulama Hatasý",
                        Message = "Þifre doðrulanamadý!"    
                    });
                    return View(model);
                }

                var user = new User(modelUser.NameSurname, modelUser.Email,modelUser.MobilePhone1,modelUser.MobilePhone2,modelUser.Gender,modelUser.Username,modelUser.Password,modelUser.Birthday,false,true,false,date,date);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("SessionStartTime", date.ToString());
                HttpContext.Session.SetInt32("IsAdmin", user.IsAdmin ? 1 : 0);

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
