using ProfileProject.Models;
using ProfileProject.Services.GeneralServices;
using System.Security.Cryptography;
using System.Text;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Services.LoginServices
{
    public class LoginService : ILoginService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeneralService generalService;

        public LoginService( ApplicationDbContext context, IGeneralService generalService)
        {
            _context = context;
            this.generalService = generalService;
        }

        public bool LoginControl(LoginModel model)
        {
            var findUser = _context.Users.FirstOrDefault(a =>
                (a.Username == model.Username || a.Email == model.Username) &&
                !a.IsDeleted &&
                a.IsActive);

            if (findUser != null)
            {
                string hashedPassword = generalService.SetHash(model.Password);
                return hashedPassword == findUser.Password;
            }
            else
            {
                return false;
            }
        }

        public User GetUserData(LoginModel model)
        {
            var findUser = _context.Users.First(a => (a.Username == model.Username || a.Email == model.Username) && !a.IsDeleted && a.IsActive);
            return findUser;
        }

        public bool ChangePassword(ChangePasswordModel model)
        {
            var findUser = _context.Users.FirstOrDefault(a => (a.Username == model.Username) && !a.IsDeleted && a.IsActive && a.Password == generalService.SetHash(model.OldPassword));
            if (findUser != null)
            {
                findUser.Password = generalService.SetHash(model.Password);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

    }
}
