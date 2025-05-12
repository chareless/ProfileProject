using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Controllers
{
    [ServiceFilter(typeof(AuthAdminFilter))]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IGeneralService generalService;
        
        public AdminController(ILogger<AdminController> logger,ApplicationDbContext context,IGeneralService generalService)
        {
            _logger = logger;
            _context = context;
            this.generalService = generalService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult User(int id)
        {
            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).Include(a=>a.Socials).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                ViewData["User"] = user.NameSurname;
                return View(user);
            }

            else
                return NotFound();
        }

        public IActionResult UserList(string? Admin, string? Active, string? Deleted)
        {
            var userList = _context.Users.
                Where(a=>Admin != null ? a.IsAdmin == (Admin == "1") : true ).
                Where(a=> Active != null ? a.IsActive == (Active == "1") : true ).
                Where(a=> Deleted != null ? a.IsDeleted == (Deleted == "1") : true )
                
                
                .ToList();
            if (userList != null)
            {
                return View(userList);
            }

            else
                return NotFound();
        }

        public IActionResult UserEdit(int id)
        {
            var user = _context.Users.FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                return View(user);
            }

            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserEdit(User model)
        {
            var user = _context.Users.FirstOrDefault(a => a.Id == model.Id);
            if (user != null)
            {
                user.IsActive = model.IsActive;
                user.IsDeleted = model.IsDeleted;
                user.IsAdmin = model.IsAdmin;
                user.UpdateWhen = generalService.GetCurrentDate();
                _context.Update(user);
                _context.SaveChanges();
                return RedirectToAction("UserList", "Admin");
            }

            else
                return NotFound();
        }

        public IActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var modelUser = model.User;
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == modelUser.Username && u.Email == modelUser.Email);

                if (existingUser != null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatalý Kullanýcý",
                        Message = "Bu kullanýcý adý veya email adresi zaten alýnmýþ!"
                    });
                    return View(model);
                }

                var date = generalService.GetCurrentDate();

                modelUser.Password = generalService.SetHash(modelUser.Password);
                model.ConfirmPassword = generalService.SetHash(model.ConfirmPassword);

                if (modelUser.Password != model.ConfirmPassword)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Þifre Doðrulama Hatasý",
                        Message = "Þifre doðrulanamadý!"
                    });
                    return View(model);
                }

                var user = new User(modelUser.NameSurname, modelUser.Email, modelUser.MobilePhone1, modelUser.MobilePhone2, modelUser.Gender, modelUser.Username, modelUser.Password, modelUser.Birthday, false, true, false, date, date);

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
