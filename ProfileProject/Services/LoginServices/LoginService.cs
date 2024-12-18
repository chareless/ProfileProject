using System.Security.Cryptography;
using System.Text;
using static ProfileProject.Models.LoginModels;

namespace ProfileProject.Services.LoginServices
{
    public class LoginService
    {
        private readonly ILoginService loginService;
        private readonly ApplicationDbContext _context;

        public LoginService(ILoginService loginService, ApplicationDbContext context)
        {
            this.loginService = loginService;
            _context = context;
        }

        public bool LoginControl(LoginModel model)
        {
            var findUser = _context.Users.FirstOrDefault(a=>a.Username ==model.Username);
            if (findUser != null)
                return SetHash(model.Password) == findUser.Password;
            else
                return false;
        }

        public string SetHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
