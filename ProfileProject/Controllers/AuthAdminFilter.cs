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

    private bool IsValidUser(AuthorizationFilterContext context)
    {
        var userID = context.HttpContext.Session.GetInt32("UserId");

        var userData = _context.Users.Where(a=>a.Id == userID && a.IsActive && !a.IsDeleted).FirstOrDefault();

        if (userData != null)
        {
            if (userData.IsAdmin)
            {
                context.HttpContext.Session.SetInt32("IsAdmin", 1);
                return true;
            }
            else
            {
                context.HttpContext.Session.SetInt32("IsAdmin", 0);
                return false;
            }
        }
        else
            return false;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
	{
		var isAdmin = context.HttpContext.Session.GetInt32("IsAdmin");
		if (isAdmin != 1 || !IsValidUser(context))
		{
			context.Result = new RedirectResult("/Login");
		}
	}
}
