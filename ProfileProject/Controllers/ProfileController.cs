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
                user.Educations = user.Educations.OrderBy(a => a.StartWhen).ToList();

                user.Educations = user.Educations.Where(a => !a.IsDeleted).ToList();
                user.Projects = user.Projects.Where(a => !a.IsDeleted).ToList();
                user.Certificates = user.Certificates.Where(a => !a.IsDeleted).ToList();
                user.WorkExperiences = user.WorkExperiences.Where(a => !a.IsDeleted).ToList();
                user.References = user.References.Where(a => !a.IsDeleted).ToList();
                user.Languages = user.Languages.Where(a => !a.IsDeleted).ToList();
                user.Skills = user.Skills.Where(a => !a.IsDeleted).ToList();
                user.Socials = user.Socials.Where(a => !a.IsDeleted).ToList();

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
                        Title = "Hatal� �ifre",
                        Message = "Yeni �ifreler e�le�memektedir!"
                    });
                    return View();
                }

                if (model.Password.Length < 6 )
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatal� �ifre",
                        Message = "Yeni �ifre uzunlu�u en az 6 karakter olmal�d�r!"
                    });
                    return View();
                }

                if (!loginService.ChangePassword(model))
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hatal� �ifre",
                        Message = "Eski �ifreniz hatal�d�r!"
                    });
                    return View();
                }

                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Ba�ar�l�",
                    Message = "�ifreniz ba�ar�l� bir �ekilde g�ncellenmi�tir."
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
                user.Educations = user.Educations.OrderBy(a => a.StartWhen).ToList();

                user.Educations = user.Educations.Where(a => !a.IsDeleted).ToList();
                user.Projects = user.Projects.Where(a => !a.IsDeleted).ToList();
                user.Certificates = user.Certificates.Where(a => !a.IsDeleted).ToList();
                user.WorkExperiences = user.WorkExperiences.Where(a => !a.IsDeleted).ToList();
                user.References = user.References.Where(a => !a.IsDeleted).ToList();
                user.Languages = user.Languages.Where(a => !a.IsDeleted).ToList();
                user.Skills = user.Skills.Where(a => !a.IsDeleted).ToList();
                user.Socials = user.Socials.Where(a => !a.IsDeleted).ToList();

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
                user.Educations = user.Educations.OrderBy(a => a.StartWhen).ToList();

                user.Educations = user.Educations.Where(a => !a.IsDeleted).ToList();
                user.Projects = user.Projects.Where(a => !a.IsDeleted).ToList();
                user.Certificates = user.Certificates.Where(a => !a.IsDeleted).ToList();
                user.WorkExperiences = user.WorkExperiences.Where(a => !a.IsDeleted).ToList();
                user.References = user.References.Where(a => !a.IsDeleted).ToList();
                user.Languages = user.Languages.Where(a => !a.IsDeleted).ToList();
                user.Skills = user.Skills.Where(a => !a.IsDeleted).ToList();
                user.Socials = user.Socials.Where(a => !a.IsDeleted).ToList();

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

                    // Daha �nce y�klenmi� di�er resimleri sil
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
                .FirstOrDefault(a => a.Id == model.Id);

            if (user == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Ba�ar�l�",
                    Message = "Kullan�c� bilgileriniz ba�ar�l� bir �ekilde g�ncellenmi�tir."
                });
                return NotFound();
            }

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

            var id = HttpContext.Session.GetInt32("UserId");
            if (id == model.Id)
            {
                _context.SaveChanges();
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Ba�ar�l�",
                    Message = "Kullan�c� bilgileriniz ba�ar�l� bir �ekilde g�ncellenmi�tir."
                });
                return View(user);
            }
            else
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullan�c� bilgisi hatal�."
                });
                return NoContent();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteInformation(int id, string Type)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (Type == "Education")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.Educations.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Educations.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "E�itim bilgisi ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "E�itim bilgisi bulunamad�."
                    });
                }
            }
            else if (Type == "Work")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.WorkExperiences.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.WorkExperiences.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "�al��ma bilgisi ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "�al��ma bilgisi bulunamad�."
                    });
                }
            }
            else if (Type == "Skill")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.Skills.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Skills.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "Teknik beceri ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Teknik beceri bulunamad�."
                    });
                }
            }
            else if (Type == "Social")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.Socials.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Socials.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "Sosyal a� ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Sosyal a� bulunamad�."
                    });
                }
            }
            else if (Type == "Reference")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.References.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.References.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "Referans bilgisi ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Referans bilgisi bulunamad�."
                    });
                }
            }
            else if (Type == "Language")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.Languages.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Languages.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "Dil ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Dil bulunamad�."
                    });
                }
            }
            else if (Type == "Certificate")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.Certificates.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Certificates.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "Sertifika ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Sertifika bulunamad�."
                    });
                }
            }
            else if (Type == "Project")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullan�c� bulunamad�."
                    });
                    return NoContent();
                }

                var data = _context.Projects.FirstOrDefault(a => a.Id == id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Projects.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Ba�ar�l�",
                        Message = "Proje ba�ar�l� bir �ekilde silinmi�tir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Proje bulunamad�."
                    });
                }
            }
            else
            {

            }
            return RedirectToAction("Edit", "Profile", new { id = userId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEducations(string educationsJson)
        {
            var dtoList = JsonConvert.DeserializeObject<List<EducationDto>>(educationsJson);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullan�c� bulunamad�."
                });
                return NoContent();
            }

            var educationList = dtoList?.Select(e => new Education
            {
                Title = e.Title,
                Name = e.Name,
                GradePoint = e.GradePoint,
                Information = e.Information,
                StartWhen = DateOnly.Parse(e.StartWhen),
                EndWhen = string.IsNullOrEmpty(e.EndWhen) ? null : DateOnly.Parse(e.EndWhen),
                UserId = userId.Value,
                CreateWhen = DateTime.Now,
                UpdateWhen = DateTime.Now,
                IsDeleted = false
            }).ToList();

            if(educationList != null)
            {
                _context.Educations.AddRange(educationList);
                _context.SaveChanges();

                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Ba�ar�l�",
                    Message = "E�itim bilgileriniz ba�ar�l� bir �ekilde g�ncellenmi�tir."
                });
            }
            else
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Hata",
                    Message = "E�itim bilgisi bulunamad�."
                });
            }
          

            return Json(new { redirectUrl = Url.Action("Edit", "Profile", new { id = userId }) });
        }
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveWorks(string worksJson)
        {
            var dtoList = JsonConvert.DeserializeObject<List<WorkDto>>(worksJson);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullan�c� bulunamad�."
                });
                return NoContent();
            }

            var workList = dtoList?.Select(e => new WorkExperience
            {
                Title = e.Position,
                CompanyName = e.Company,
                Information = e.Information,
                StartWhen = DateOnly.Parse(e.StartWhen),
                EndWhen = string.IsNullOrEmpty(e.EndWhen) ? null : DateOnly.Parse(e.EndWhen),
                UserId = userId.Value,
                CreateWhen = DateTime.Now,
                UpdateWhen = DateTime.Now,
                IsDeleted = false
            }).ToList();

            if (workList != null)
            {
                _context.WorkExperiences.AddRange(workList);
                _context.SaveChanges();

                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Ba�ar�l�",
                    Message = "�al��ma bilgileriniz ba�ar�l� bir �ekilde g�ncellenmi�tir."
                });
            }
            else
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Hata",
                    Message = "�al��ma bilgisi bulunamad�."
                });
            }


            return Json(new { redirectUrl = Url.Action("Edit", "Profile", new { id = userId }) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSkills(string skillsJson)
        {
            var dtoList = JsonConvert.DeserializeObject<List<SkillDto>>(skillsJson);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullan�c� bulunamad�."
                });
                return NoContent();
            }

            var skillList = dtoList?.Select(e => new Skill
            {
                Title = e.Title,
                Information = e.Information,
                UserId = userId.Value,
                CreateWhen = DateTime.Now,
                UpdateWhen = DateTime.Now,
                IsDeleted = false
            }).ToList();

            if (skillList != null)
            {
                _context.Skills.AddRange(skillList);
                _context.SaveChanges();

                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Ba�ar�l�",
                    Message = "Teknik becerileriniz ba�ar�l� bir �ekilde g�ncellenmi�tir."
                });
            }
            else
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Hata",
                    Message = "Teknik beceri bulunamad�."
                });
            }


            return Json(new { redirectUrl = Url.Action("Edit", "Profile", new { id = userId }) });
        }

        //EKLENECEKLER

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
