using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProfileProject.Services.LoginServices;

var builder = WebApplication.CreateBuilder(args);

// Connection string'i appsettings.json'dan al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext'i yapýlandýr
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Diðer servisleri ekleyin
builder.Services.AddControllersWithViews();


builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<AuthAdminFilter>();
builder.Services.AddScoped<AuthRegisterFilter>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(12);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware'leri yapýlandýr
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
