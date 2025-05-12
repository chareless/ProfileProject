using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using ProfileProject.Services.LoginServices;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Controllers
{
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
                .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).Include(a => a.Socials).FirstOrDefault(a => a.Id == id);
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
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).Include(a => a.Socials).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                var visitor = new UserVisit(user.Id);
                _context.UserVisits.Add(visitor);
                _context.SaveChanges();
                user.VisitorCount = _context.UserVisits.Where(a=>a.UserId == user.Id).Count();
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
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).Include(a => a.Socials).FirstOrDefault(a => a.Id == id);
            if (user != null)
            {
                return View(user);
            }

            else
                return NotFound();
        }

        [HttpPost]
        public async Task<string?> PictureUpload(IFormFile formFile, int UserID)
        {
            if (formFile != null)
            {
                string originalExtension = Path.GetExtension(formFile.FileName);
                string sanitizedFileName = GeneralService.SanitizeFileName(Path.GetFileNameWithoutExtension(formFile.FileName));
                string newFileName = sanitizedFileName + originalExtension;

                string folderPath = Path.Combine("wwwroot", "Profiles", UserID.ToString(), "Images");
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath, newFileName);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // Daha önce yüklenmiþ diðer resimleri sil
                    var imageFiles = Directory.GetFiles(folderPath);
                    foreach (var file in imageFiles)
                    {
                        string fileName = Path.GetFileName(file);
                        if (!fileName.Equals(newFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            System.IO.File.Delete(file);
                        }
                    }

                    var user = _context.Users.FirstOrDefault(a => a.Id == UserID);
                    if (user != null)
                    {
                        user.Picture = @$"/Profiles/{UserID}/Images/" + newFileName;
                        _context.SaveChanges();
                    }
                    else
                    {
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        return null;
                    }


                    return newFileName;
                }
                catch (Exception e)
                {
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    throw;
                }
            }

            return null;
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users
                .Include(a => a.Projects)
                .Include(a => a.Certificates)
                .Include(a => a.WorkExperiences)
                .Include(a => a.Educations)
                .Include(a => a.References)
                .Include(a => a.Languages)
                .Include(a => a.Skills)
                .Include(a => a.Socials)
                .FirstOrDefault(a => a.Id == model.Id);

            if (user == null)
                return NotFound();

            user.NameSurname = model.NameSurname;
            user.Email = model.Email;
            user.MobilePhone1 = model.MobilePhone1;
            user.MobilePhone2 = model.MobilePhone2;
            user.Birthday = model.Birthday;
            user.Job = model.Job;
            user.About = model.About;
            user.Marriage = model.Marriage;
            user.DrivingLicense = model.DrivingLicense;
            user.Military = model.Military;
            user.Hobbies = model.Hobbies;
            user.IsActive = model.IsActive;
            user.UpdateWhen = GeneralService.GetCurrentDateStatic();

            _context.Educations.RemoveRange(user.Educations);
            _context.WorkExperiences.RemoveRange(user.WorkExperiences);
            _context.Projects.RemoveRange(user.Projects);
            _context.Certificates.RemoveRange(user.Certificates);
            _context.References.RemoveRange(user.References);
            _context.Languages.RemoveRange(user.Languages);
            _context.Skills.RemoveRange(user.Skills);
            _context.Socials.RemoveRange(user.Socials);

            user.Educations = model.Educations;
            user.WorkExperiences = model.WorkExperiences;
            user.Projects = model.Projects;
            user.Certificates = model.Certificates;
            user.References = model.References;
            user.Languages = model.Languages;
            user.Skills = model.Skills;
            user.Socials = model.Socials;

            _context.SaveChanges();

            TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
            {
                AlertType = "success",
                Title = "Baþarýlý",
                Message = "Kullanýcý bilgileriniz baþarýlý bir þekilde güncellenmiþtir."
            });

            var id = HttpContext.Session.GetInt32("UserId");
            if(id == model.Id)
                return RedirectToAction("Index", "Profile");
            else
                return RedirectToAction($"User/{id}", "Profile");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
