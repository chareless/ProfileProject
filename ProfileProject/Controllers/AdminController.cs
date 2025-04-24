using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;

namespace ProfileProject.Controllers
{
    //[ServiceFilter(typeof(AuthAdminFilter))]
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
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                return View(user);
            }

            else
                return NotFound();
        }

        public IActionResult UserList()
        {
            var userList = _context.Users.ToList();
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
