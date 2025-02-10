namespace ProfileProject.Services.GeneralServices
{
    public interface IGeneralService
    {
        DateTime GetCurrentDate();
        string SetHash(string password);
    }
}
