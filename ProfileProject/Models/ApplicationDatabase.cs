using ProfileProject.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Education> Educations { get; set; }
    public DbSet<WorkExperience> WorkExperiences { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Reference> References { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<LoginControl> LoginControls { get; set; }
    public DbSet<UserAccessLog> UserAccessLogs { get; set; }
    public DbSet<UserVisit> UserVisits { get; set; }
    public DbSet<UserTheme> UserThemes { get; set; }
}
