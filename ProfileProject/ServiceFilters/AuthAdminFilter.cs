using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

public class AuthAdminFilter : Controller, IAuthorizationFilter
{
	private readonly ILogger<AuthAdminFilter> _logger;
	private readonly IConfiguration _configuration;
	private readonly ApplicationDbContext _context;
	private IHttpContextAccessor _accessor;


	public AuthAdminFilter(ILogger<AuthAdminFilter> logger, IConfiguration Configuration, ApplicationDbContext context, IHttpContextAccessor accessor)
	{
		_logger = logger;
		_configuration = Configuration;
		_context = context;
		_accessor = accessor;
	}

    private bool IsValidAdminUser(AuthorizationFilterContext context)
    {
        var userID = context.HttpContext.Session.GetInt32("UserId");

        var userData = _context.Users.FirstOrDefault(a => a.Id == userID && a.IsActive && !a.IsDeleted);

        if (userData != null && userData.IsAdmin)
        {
            context.HttpContext.Session.SetInt32("IsAdmin", 1);
            return userData.IsAdmin;
        }
        else
        {
            context.HttpContext.Session.SetInt32("IsAdmin", 0);
            return false;
        }
    }

    public void OnAuthorization(AuthorizationFilterContext context)
	{
        if (!IsValidAdminUser(context))
        {
            try
            {
                HttpContext?.Session?.Remove("Username");
                HttpContext?.Session?.Remove("Email");
                HttpContext?.Session?.Remove("UserId");
                HttpContext?.Session?.Remove("SessionStartTime");
                HttpContext?.Session?.Remove("IsAdmin");
                context.Result = new RedirectResult("/Login");
            }
            catch
            {
                context.Result = new RedirectResult("/Login");
            }
        }
    }
}
