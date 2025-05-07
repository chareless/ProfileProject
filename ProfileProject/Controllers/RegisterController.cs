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
                        Title = "Hatal� Kullan�c�",
                        Message = "Bu kullan�c� ad� zaten al�nm��!"
                    });
                    return View(model);
                }

                if (existingMail != null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatal� Kullan�c�",
                        Message = "Bu email adresi zaten al�nm��!"
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
                        Title = "�ifre Do�rulama Hatas�",
                        Message = "�ifre do�rulanamad�!"    
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
