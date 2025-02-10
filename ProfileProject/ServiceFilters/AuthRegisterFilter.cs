using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

public class AuthRegisterFilter : Controller, IAuthorizationFilter
{
	private readonly ILogger<AuthRegisterFilter> _logger;
	private readonly IConfiguration _configuration;
	private readonly ApplicationDbContext _context;
	private IHttpContextAccessor _accessor;


	public AuthRegisterFilter(ILogger<AuthRegisterFilter> logger, IConfiguration Configuration, ApplicationDbContext context, IHttpContextAccessor accessor)
	{
		_logger = logger;
		_configuration = Configuration;
		_context = context;
		_accessor = accessor;
	}

    private bool IsValidUser(AuthorizationFilterContext context)
    {
        var userID = context.HttpContext.Session.GetInt32("UserId");

        return _context.Users.Any(a => a.Id == userID && !a.IsDeleted && a.IsActive);
    }

    public void OnAuthorization(AuthorizationFilterContext context)
	{
        if (!IsValidUser(context))
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
