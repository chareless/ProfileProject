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

        public static DateTime GetCurrentDateStatic()
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

        public static string SanitizeFileName(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c.ToString(), "_");
            }

            return fileName.Replace("+", "_")
                           .Replace(" ", "_")
                           .Replace("&", "_")
                           .Replace("#", "_")
                           .Replace("?", "_")
                           .Replace("=", "_")
                           .Replace("%", "_");
        }

    }
}
