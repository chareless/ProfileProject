using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProfileProject.Models;
using ProfileProject.Services.LoginServices;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Controllers
{
    [ServiceFilter(typeof(AuthRegisterFilter))]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILoginService loginService;

        public ProfileController(ILogger<ProfileController> logger,ApplicationDbContext context, ILoginService loginService)
        {
            _logger = logger;
            _context = context;
            this.loginService = loginService;
        }

        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                return View(user);
            }

            else
                return NotFound();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            model.Username = HttpContext.Session.GetString("Username") ?? "";
            if (ModelState.IsValid)
            {
                if(model.Password != model.ConfirmPassword)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Þifre",
                        Message = "Yeni þifreler eþleþmemektedir!"
                    });
                    return View();
                }

                if (model.Password.Length < 6 )
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Þifre",
                        Message = "Yeni þifre uzunluðu en az 6 karakter olmalýdýr!"
                    });
                    return View();
                }

                if (!loginService.ChangePassword(model))
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Þifre",
                        Message = "Eski þifreniz hatalýdýr!"
                    });
                    return View();
                }

                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Baþarýlý",
                    Message = "Þifreniz baþarýlý bir þekilde güncellenmiþtir."
                });
                return RedirectToAction("Index", "Profile");
            }

            return View(model);
        }

        public IActionResult User(int id)
        {
            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                user.VisitorCount++;
                _context.Update(user);
                _context.SaveChanges();
                return View(user);
            }
               
            else
                return NotFound();
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                return View(user);
            }

            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model)
        {
            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).FirstOrDefault(a => a.Id == model.Id);
            if (user != null)
            {
                return View(user);
            }

            else
                return NotFound();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
