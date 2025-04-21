using Newtonsoft.Json;
using ProfileProject.Services.GeneralServices;

namespace ProfileProject.Core
{
    public class Logg
    {
        public static string BasePath { get; set; }
        private static readonly object SyncRoot = new();
        public static string Filename = "";
        public static bool Tracing = (System.Configuration.ConfigurationManager.AppSettings["Tracing"] != null && System.Configuration.ConfigurationManager.AppSettings["Tracing"] == "1");
        private static IHttpContextAccessor _httpContextAccessor;
        private static IWebHostEnvironment _env;

        public static void Configure(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        private static int? GetSessionId()
        {
            return _httpContextAccessor?.HttpContext?.Session?.GetInt32("UserId");
        }

        private static string? GetSessionUser()
        {
            return _httpContextAccessor?.HttpContext?.Session?.GetString("Username");
        }

        private static string? GetEnvironment()
        {
            return _env.IsDevelopment() ? "DEBUG" : "RELEASE";
        }

        private static string? GetIP()
        {
            return _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        private static DateTime GetCurrentDate()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"));
        }

        public static void SetBasePath(string path)
        {
            BasePath = path;
        }

        public static void Debug(string text, string section = "profile_project_activity")
        {
            lock (SyncRoot)
            {
                var logLine =
                    $"{GetCurrentDate():yyyy-MM-dd HH:mm:ss.ffff} | {GetEnvironment()} | " +
                    $"UserID: {GetSessionId()} | " +
                    $"User: {GetSessionUser()} | " +
                    $"IP: {GetIP()} | " +
                    $"{text}" +
                    "\r\n";

                var path = GetPath(subDir: section);
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("GetPath returned null or empty path");
                }
                File.AppendAllText(path, logLine);
            }
        }

        public static void Debug(LogModel model, string text, bool error = false, string section = "profile_project_activity")
        {
            lock (SyncRoot)
            {
                var jsonBody = JsonConvert.SerializeObject(model.Body, Formatting.Indented);
                var logLine =
                    $"{model.Time.ToString("yyyy-MM-dd HH:mm:ss.ffff")}|" +
                    $"{(model.Error ? "E" : " ")}| {model.LogType ?? GetEnvironment(),-5} | " +
                    $"{model.RemoteIp,-14} | " +
                    $"{(!string.IsNullOrEmpty(model.User) ? model.User : ""),-8} | " +
                    $"{model.RequestType,-4} | {model.RequestPage} :: {text} | " +
                    $"BODY : {jsonBody} | CONTROLLER : {model.Controller} | ACTION : {model.Action}" +
                    "\r\n";

                File.AppendAllText(GetPath(error, subDir: section), logLine);
            }
        }

        public static void DebugCustom(LogModel model, string filename, string text, bool error = false)
        {
            lock (SyncRoot)
            {
                var logLine = string.Format("{0}|{1}| {2,-5} | {3,-14} | {4,-8} | {5,-4} | {6} :: {7}" + "\r\n",
                                model.Time.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                                model.Error ? "E" : " ",
                                model.LogType ?? GetEnvironment(),
                                model.RemoteIp,
                                !string.IsNullOrEmpty(model.User) ? model.User : "",
                                model.RequestType,
                                model.RequestPage,
                                text
                                );

                File.AppendAllText(GetPath(filename: filename, subDir: "server"), logLine);
            }
        }

        public static void Exception(LogModel model, string text, Exception ex)
        {
            lock (SyncRoot)
            {
                var logLine = string.Format("{0}|{1}| {2,-5} | {3,-14} | {4,-8} | {5,-4} | {6} :: {7}" + "\r\n",
                                model.Time.ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                                model.Error ? "E" : " ",
                                model.LogType ?? GetEnvironment(),
                                model.RemoteIp,
                                !string.IsNullOrEmpty(model.User) ? model.User : "",
                                model.RequestType,
                                model.RequestPage,
                                string.Format(text + " Reason: {0}{1}", ex.Message, ex.InnerException != null ? ", Detail: " + ex.InnerException.Message : "")
                                );

                File.AppendAllText(GetPath(true), logLine);
            }
        }

        public static void Exception(string text, Exception ex, string section = "")
        {
            lock (SyncRoot)
            {
                File.AppendAllText(GetPath(subDir: section), string.Format("{0}| {1}\r\nReason: {2}{3}\r\n\r\n", GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd HH:mm:ss.ffff"), text, ex.Message, ex.InnerException != null ? "\r\nDetail: " + ex.InnerException.Message : ""));
            }
        }

        public static void ApplicationError(string errorMessage)
        {
            lock (SyncRoot)
            {
                File.AppendAllText(GetPath(true), string.Format("{0}|{1}\r\n", GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd HH:mm:ss.ffff"), errorMessage));
            }
        }

        private static string GetPath(bool errorFile = false, string filename = null, string subDir = "profile_project_activity")
        {
            if (string.IsNullOrEmpty(filename))
                filename = GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd") + (errorFile ? "_E" : "");
            else
                filename = GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd") + "_" + filename;

            return CheckDirectory(subDir) + filename + ".log";
        }

        private static string CheckDirectory(string subDir)
        {

            var filePath = Path.Combine(BasePath, "log", subDir);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return filePath + "\\";
        }


        public static void Service(string ip, string text, bool error = false)
        {
            lock (SyncRoot)
            {
                if (string.IsNullOrEmpty(BasePath))
                    BasePath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\";

                var logLine = string.Format("{0}|{1}| {2,-5} | {3,-15} | {4}" + "\r\n",
                                            GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                                            error ? "E" : " ",
                                            GetEnvironment(),
                                            ip,
                                            text
                                            );

                File.AppendAllText(Path.Combine(BasePath, GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd") + ".log"), logLine);
            }
        }



        #region DEBUG

        public static void Debug(LogModel model, string format, params object[] paramList)
        {
            model.LogType = "DEBUG";
            Debug(model, string.Format(format, paramList));
        }

        public static void Debug(LogModel model, Exception ex)
        {
            model.LogType = "DEBUG";
            model.Error = true;
            Debug(model, "Exception! daha fazla detay için _E uzantılı log dosyasına bakınız!");
            Debug(model, string.Format("Reason: {0} {1}", ex.Message, ex.InnerException != null ? ", Detay: " + ex.InnerException : ""), true);
        }

        #endregion

        #region TRACE

        public static void Trace(LogModel model, string format, params object[] paramList)
        {
            if (!Tracing) return;

            Trace(model, string.Format(format, paramList));
        }

        public static void Trace(LogModel model, string text)
        {
            if (!Tracing) return;

            model.LogType = "TRACE";
            Debug(model, text);
        }

        #endregion




        public static void Custom(string filename, string ip, string account, string text)
        {
            lock (SyncRoot)
            {
                var logLine = string.Format("{0}|{1}| {2,-5} | {3}" + "\r\n",
                                GeneralService.GetCurrentDateStatic().ToString("yyyy-MM-dd HH:mm:ss.ffff"),
                                ip,
                                !string.IsNullOrEmpty(account) ? account : "",
                                text
                                );

                File.AppendAllText(GetPath(filename: filename), logLine);
            }
        }
    }

    public class LogModel
    {
        public string RequestType { get; set; }
        public string RemoteIp { get; set; }
        public DateTime Time { get; set; }
        public string LogType { get; set; }
        public string RequestPage { get; set; }
        public string User { get; set; }
        public bool Error { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Body { get; set; }
    }
}