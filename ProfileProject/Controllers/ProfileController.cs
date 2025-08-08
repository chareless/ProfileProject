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
                user.Educations = user.Educations.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.Projects = user.Projects.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Certificates = user.Certificates.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.WorkExperiences = user.WorkExperiences.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.References = user.References.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Languages = user.Languages.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Skills = user.Skills.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Socials = user.Socials.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();

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

        [Route("Profile/User/{username}")]
        public IActionResult User(string username)
        {
            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).Include(a => a.Socials).FirstOrDefault(a => a.Username == username);
            if (user != null)
            {
                user.Educations = user.Educations.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.Projects = user.Projects.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Certificates = user.Certificates.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.WorkExperiences = user.WorkExperiences.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.References = user.References.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Languages = user.Languages.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Skills = user.Skills.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Socials = user.Socials.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();

                if(user.Username != username)
                {
                    var visitor = new UserVisit(user.Id);
                    _context.UserVisits.Add(visitor);
                    _context.SaveChanges();
                    user.VisitorCount = _context.UserVisits.Where(a => a.UserId == user.Id).Count();
                    _context.Update(user);
                    _context.SaveChanges();
                }
              
                return View(user);
            }
               
            else
                return NotFound();
        }

        [Route("Profile/Edit/{username}")]
        public IActionResult Edit(string username)
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            var userName = HttpContext.Session.GetString("Username");

            var userCheck = _context.Users
                .FirstOrDefault(a => a.Username == userName && userID == a.Id);

            if (userCheck == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullanýcý bilgisi hatalý."
                });
                return RedirectToAction("Index", "Profile");
            }

            var user = _context.Users.Include(a => a.Projects).Include(a => a.Certificates).Include(a => a.WorkExperiences).Include(a => a.Educations)
                 .Include(a => a.References).Include(a => a.Languages).Include(a => a.Skills).Include(a => a.Socials).FirstOrDefault(a => a.Username == username);
            if (user != null)
            {
                user.Educations = user.Educations.OrderBy(a => a.StartWhen).ToList();

                user.Educations = user.Educations.Where(a => !a.IsDeleted).OrderByDescending(a=>a.StartWhen).ToList();
                user.Projects = user.Projects.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Certificates = user.Certificates.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.WorkExperiences = user.WorkExperiences.Where(a => !a.IsDeleted).OrderByDescending(a => a.StartWhen).ToList();
                user.References = user.References.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Languages = user.Languages.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Skills = user.Skills.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();
                user.Socials = user.Socials.Where(a => !a.IsDeleted).OrderBy(a => a.Order).ToList();

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

            var id = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users
                .FirstOrDefault(a => a.Id == model.Id && a.Id == id);

            if (user == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullanýcý bilgisi hatalý."
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
            user.Country = model.Country;
            user.City = model.City;
            user.District = model.District;
            user.UpdateWhen = GeneralService.GetCurrentDateStatic();

            if (id == model.Id)
            {
                _context.SaveChanges();
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "success",
                    Title = "Baþarýlý",
                    Message = "Kullanýcý bilgileriniz baþarýlý bir þekilde güncellenmiþtir."
                });
                return View(user);
            }
            else
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullanýcý bilgisi hatalý."
                });
                return NoContent();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteInformation(int id, string Type)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");
            if (Type == "Education")
            {
                if (userId == null)
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.Educations.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Educations.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Eðitim bilgisi baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Eðitim bilgisi bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.WorkExperiences.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.WorkExperiences.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Çalýþma bilgisi baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Çalýþma bilgisi bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.Skills.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Skills.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Teknik beceri baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Teknik beceri bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.Socials.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Socials.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Sosyal að baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Sosyal að bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.References.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.References.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Referans bilgisi baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Referans bilgisi bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.Languages.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Languages.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Dil baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Dil bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.Certificates.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Certificates.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Sertifika baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Sertifika bulunamadý."
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
                        Message = "Kullanýcý bulunamadý."
                    });
                    return NoContent();
                }

                var data = _context.Projects.FirstOrDefault(a => a.Id == id && a.UserId == userId);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdateWhen = GeneralService.GetCurrentDateStatic();
                    _context.Projects.Update(data);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Proje baþarýlý bir þekilde silinmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Proje bulunamadý."
                    });
                }
            }
            else
            {

            }
            return RedirectToAction("Edit", "Profile", new { username = username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveInformation(string data,string dataType)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var username = HttpContext.Session.GetString("Username");
            if (userId == null)
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Kullanýcý bulunamadý."
                });
                return NoContent();
            }

            if(dataType == "Education")
            {
                var dtoList = JsonConvert.DeserializeObject<List<EducationDto>>(data);
                var educationList = dtoList?.Select(e => new Education
                {
                    Id = e.Id,
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

                if (educationList != null && educationList.Count != 0)
                {
                    if(educationList.Any(a=>a.Id != 0) && educationList[0].UserId == userId)
                        _context.Educations.Update(educationList[0]);
                    else
                        _context.Educations.AddRange(educationList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Eðitim bilgileriniz baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Eðitim bilgisi bulunamadý."
                    });
                }
            }
            else if (dataType == "Work")
            {
                var dtoList = JsonConvert.DeserializeObject<List<WorkDto>>(data);
                var workList = dtoList?.Select(e => new WorkExperience
                {
                    Id = e.Id,
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

                if (workList != null && workList.Count != 0)
                {
                    if (workList.Any(a => a.Id != 0) && workList[0].UserId == userId)
                        _context.WorkExperiences.Update(workList[0]);
                    else
                        _context.WorkExperiences.AddRange(workList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Çalýþma bilgileriniz baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Çalýþma bilgisi bulunamadý."
                    });
                }
            }
            else if(dataType == "Skill")
            {
                var dtoList = JsonConvert.DeserializeObject<List<SkillDto>>(data);
                var skillList = dtoList?.Select(e => new Skill
                {
                    Id = e.Id,
                    Title = e.Title,
                    Information = e.Information,
                    Order = e.Order,
                    UserId = userId.Value,
                    CreateWhen = DateTime.Now,
                    UpdateWhen = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                if (skillList != null && skillList.Count != 0)
                {
                    if (skillList.Any(a => a.Id != 0) && skillList[0].UserId == userId)
                        _context.Skills.Update(skillList[0]);
                    else
                        _context.Skills.AddRange(skillList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Teknik becerileriniz baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Teknik beceri bulunamadý."
                    });
                }
            }
            else if(dataType == "Social")
            {
                var dtoList = JsonConvert.DeserializeObject<List<SocialDto>>(data);
                var socialList = dtoList?.Select(e => new Social
                {
                    Id = e.Id,
                    Name = e.Name,
                    Link = e.Link,
                    Order = e.Order,
                    UserId = userId.Value,
                    CreateWhen = DateTime.Now,
                    UpdateWhen = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                if (socialList != null)
                {
                    if (socialList.Any(a => a.Id != 0) && socialList[0].UserId == userId)
                        _context.Socials.Update(socialList[0]);
                    else
                        _context.Socials.AddRange(socialList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Sosyal aðlar baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Sosyal að bulunamadý."
                    });
                }
            }
            else if(dataType == "Language")
            {
                var dtoList = JsonConvert.DeserializeObject<List<LanguageDto>>(data);
                var languageList = dtoList?.Select(e => new Language
                {
                    Id = e.Id,
                    Name = e.Name,
                    Order = e.Order,
                    Information = e.Info,
                    UserId = userId.Value,
                    CreateWhen = DateTime.Now,
                    UpdateWhen = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                if (languageList != null && languageList.Count != 0)
                {
                    if (languageList.Any(a => a.Id != 0) && languageList[0].UserId == userId)
                        _context.Languages.Update(languageList[0]);
                    else
                        _context.Languages.AddRange(languageList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Diller baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Dil bulunamadý."
                    });
                }
            }
            else if (dataType == "Reference")
            {
                var dtoList = JsonConvert.DeserializeObject<List<ReferenceDto>>(data);
                var referenceList = dtoList?.Select(e => new Reference
                {
                    Id = e.Id,
                    Name = e.Name,
                    Information = e.Info,
                    CompanyName = e.Company,
                    Email = e.Mail,
                    MobilePhone = e.Phone,
                    Title = e.Title,
                    Order = e.Order,
                    UserId = userId.Value,
                    CreateWhen = DateTime.Now,
                    UpdateWhen = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                if (referenceList != null && referenceList.Count != 0)
                {
                    if (referenceList.Any(a => a.Id != 0) && referenceList[0].UserId == userId)
                        _context.References.Update(referenceList[0]);
                    else
                        _context.References.AddRange(referenceList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Referans bilgileri baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Referans bilgisi bulunamadý."
                    });
                }
            }
            else if (dataType == "Certificate")
            {
                var dtoList = JsonConvert.DeserializeObject<List<CertificateDto>>(data);
                var certificateList = dtoList?.Select(e => new Certificate
                {
                    Id = e.Id,
                    CompanyName = e.Company,
                    StartWhen = DateOnly.Parse(e.StartWhen),
                    EndWhen = string.IsNullOrEmpty(e.EndWhen) ? null : DateOnly.Parse(e.EndWhen),
                    Information = e.Info,
                    UserId = userId.Value,
                    CreateWhen = DateTime.Now,
                    UpdateWhen = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                if (certificateList != null && certificateList.Count != 0)
                {
                    if (certificateList.Any(a => a.Id != 0) && certificateList[0].UserId == userId)
                        _context.Certificates.Update(certificateList[0]);
                    else
                        _context.Certificates.AddRange(certificateList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Sertifikalar baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Sertifika bilgisi bulunamadý."
                    });
                }
            }
            else if (dataType == "Project")
            {
                var dtoList = JsonConvert.DeserializeObject<List<ProjectDto>>(data);
                var projectList = dtoList?.Select(e => new Project
                {
                    Id = e.Id,
                    Name = e.Name,
                    Information = e.Info,
                    Order = e.Order,
                    StartWhen = string.IsNullOrEmpty(e.StartWhen) ? null : DateOnly.Parse(e.StartWhen),
                    EndWhen = string.IsNullOrEmpty(e.EndWhen) ? null : DateOnly.Parse(e.EndWhen),
                    UserId = userId.Value,
                    CreateWhen = DateTime.Now,
                    UpdateWhen = DateTime.Now,
                    IsDeleted = false
                }).ToList();

                if (projectList != null && projectList.Count != 0)
                {
                    if (projectList.Any(a => a.Id != 0) && projectList[0].UserId == userId)
                        _context.Projects.Update(projectList[0]);
                    else
                        _context.Projects.AddRange(projectList);
                    _context.SaveChanges();

                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "success",
                        Title = "Baþarýlý",
                        Message = "Projeler baþarýlý bir þekilde güncellenmiþtir."
                    });
                }
                else
                {
                    TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                    {
                        AlertType = "warning",
                        Title = "Hata",
                        Message = "Proje bulunamadý."
                    });
                }
            }
            else
            {
                TempData["AlertMessage"] = JsonConvert.SerializeObject(new AlertMessage
                {
                    AlertType = "warning",
                    Title = "Hata",
                    Message = "Genel Hata!"
                });
            }

            return Json(new { redirectUrl = Url.Action("Edit", "Profile", new { username = username }) });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
