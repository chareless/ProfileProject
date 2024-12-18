using static ProfileProject.Models.LoginModels;

namespace ProfileProject.Services.LoginServices
{
    public interface ILoginService
    {
        bool LoginControl(LoginModel model);
        string SetHash(string password);
    }
}
