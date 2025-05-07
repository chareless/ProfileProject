using ProfileProject.Models;
using static ProfileProject.Models.DataModels;

namespace ProfileProject.Services.LoginServices
{
    public interface ILoginService
    {
        bool LoginControl(LoginModel model);
        User GetUserData(LoginModel model);
        bool ChangePassword(ChangePasswordModel model);
        UserTheme? GetUserTheme(int id);
        void LoginUser(int id);
    }
}
