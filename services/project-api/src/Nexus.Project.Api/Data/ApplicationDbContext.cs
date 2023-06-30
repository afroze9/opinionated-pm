using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nexus.Persistence;
using Nexus.Persistence.Auditing;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext : AuditableDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options, auditableEntitySaveChangesInterceptor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<Project>().HasData(_projects);
        modelBuilder.Entity<TodoItem>().HasData(GetTodoItems());
    }

    public DbSet<Project> Projects => Set<Project>();
    
    private readonly List<Project> _projects = new()
    {
        new Project("Portfolio Website", Priority.Low, 1) { Id = 1 },
        new Project("Nexus Solution", Priority.High, 2) { Id = 2 },
        new Project("Nexus Libraries", Priority.High, 2) { Id = 3 },
        new Project("Nexus Tool", Priority.High, 2) { Id = 4 },
        new Project("Nexus Template", Priority.Medium, 2) { Id = 5 },
        new Project("Presentation", Priority.Critical, 2) { Id = 6 },
    };
    
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    private List<TodoItem> GetTodoItems()
    {
        TodoItem p1T1 = new () { Id = 1, Title = "Create React Site", Description = "Create a portfolio website using react", ProjectId = 1 };
        TodoItem p1T2 = new () { Id = 2, Title = "Deploy site", Description = "Deploy portfolio site to Netlify", ProjectId = 1 };
        p1T1.MarkComplete();
        p1T2.MarkComplete();

        TodoItem p2T1 = new () { Id = 3, Title = "Api Gateway", Description = "Create Api Gateway", ProjectId = 2 };
        TodoItem p2T2 = new () { Id = 4, Title = "HC Dashboard", Description = "Create Health Check Dashboard", ProjectId = 2 };
        TodoItem p2T3 = new () { Id = 5, Title = "Discovery Server", Description = "Setup Discovery Server", ProjectId = 2 };
        TodoItem p2T4 = new () { Id = 6, Title = "Company API", Description = "Create Company API", ProjectId = 2 };
        TodoItem p2T5 = new () { Id = 7, Title = "Project API", Description = "Create Project API", ProjectId = 2 };
        TodoItem p2T6 = new () { Id = 8, Title = "Documentation", Description = "Create documentation", ProjectId = 2 };
        p2T1.MarkComplete();
        p2T2.MarkComplete();
        p2T3.MarkComplete();
        p2T4.MarkComplete();
        p2T5.MarkComplete();

        TodoItem p3T1 = new () { Id = 9, Title = "Auth", Description = "Create Nexus.Auth", ProjectId = 3 };
        TodoItem p3T2 = new () { Id = 10, Title = "Common", Description = "Create Nexus.Common", ProjectId = 3 };
        TodoItem p3T3 = new () { Id = 11, Title = "Configuration", Description = "Create Nexus.Configuration", ProjectId = 3 };
        TodoItem p3T4 = new () { Id = 12, Title = "Framework.Web", Description = "Create Nexus.Framework.Web", ProjectId = 3 };
        TodoItem p3T5 = new () { Id = 13, Title = "Logs", Description = "Create Nexus.Logs", ProjectId = 3 };
        TodoItem p3T6 = new () { Id = 14, Title = "Management", Description = "Create Nexus.Management", ProjectId = 3 };
        TodoItem p3T7 = new () { Id = 15, Title = "Persistence", Description = "Create Nexus.Persistence", ProjectId = 3 };
        TodoItem p3T8 = new () { Id = 16, Title = "Telemetry", Description = "Create Nexus.Telemetry", ProjectId = 3 };
        p3T1.MarkComplete();
        p3T2.MarkComplete();
        p3T3.MarkComplete();
        p3T4.MarkComplete();
        p3T5.MarkComplete();
        p3T6.MarkComplete();
        p3T7.MarkComplete();
        p3T8.MarkComplete();

        TodoItem p4T1 = new () { Id = 17, Title = "Create Tool", Description = "Create Nexus CLI Tool", ProjectId = 4 };
        TodoItem p4T2 = new () { Id = 18, Title = "Testing", Description = "Test CLI Tool", ProjectId = 4 };
        p4T1.MarkComplete();
        p4T2.MarkComplete();
        
        TodoItem p5T1 = new () { Id = 19, Title = "Create Template", Description = "Create Nexus CLI Tool", ProjectId = 5 };
        p5T1.MarkComplete();
        
        TodoItem p6T1 = new () { Id = 20, Title = "Create Presentation", Description = "Create Presentation for Nexus", ProjectId = 6 };
        TodoItem p6T2 = new () { Id = 21, Title = "Present", Description = "Present in front of audience", ProjectId = 6 };
        p6T1.MarkComplete();

        return new List<TodoItem>
        {
            p1T1, p1T2,
            p2T1, p2T2, p2T3, p2T4, p2T5,
            p3T1, p3T2, p3T3, p3T4, p3T5, p3T6, p3T7, p3T8,
            p4T1, p4T2,
            p5T1,
            p6T1, p6T2,
        };
    }
}