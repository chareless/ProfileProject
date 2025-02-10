using System.Security.Cryptography;
using System.Text;

namespace ProfileProject.Services.GeneralServices
{
    public class GeneralService : IGeneralService
    {
        private readonly ApplicationDbContext _context;

        public GeneralService( ApplicationDbContext context)
        {
            _context = context;
        }

        public DateTime GetCurrentDate()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
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
