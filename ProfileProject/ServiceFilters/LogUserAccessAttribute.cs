
using ProfileProject.Core;
using ProfileProject.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

public class LogUserAccessAttribute : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        var httpContext = context.HttpContext;
        int? userId = httpContext.Session.GetInt32("UserId");
        var controller = context.RouteData.Values["controller"]?.ToString() ?? "";
        var action = context.RouteData.Values["action"]?.ToString() ?? "";
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "";

        var fullUrl = httpContext.Request.Path + httpContext.Request.QueryString;

        using (var scope = httpContext.RequestServices.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            //if (!env.IsDevelopment()&& !fullUrl.Contains("Layout")) // sadece production'da loglama
            if (true && !fullUrl.Contains("Layout")) 
            {
                var id = 0;
                if (fullUrl.Contains("/User/") && !fullUrl.Contains("/Admin/"))
                {
                    var list = fullUrl.Split("/User/").ToList();
                    int.TryParse(list[1],out id);
                }

                var log = new UserAccessLog(userId, controller, action, DateTime.Now, ip, fullUrl,id);

                dbContext.UserAccessLogs.Add(log);
                await dbContext.SaveChangesAsync();
                string logJson = JsonSerializer.Serialize(log);
                Logg.Debug(logJson);
            }
        }
    }
}