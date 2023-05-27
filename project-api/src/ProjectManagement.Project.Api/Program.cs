using Microsoft.EntityFrameworkCore;
using ProjectManagement.ProjectAPI.Data;
using ProjectManagement.ProjectAPI.Endpoints;
using ProjectManagement.ProjectAPI.Extensions;

CoreWebApplicationBuilder.BuildConfigureAndRun(args,
    (services, configuration) => { services.RegisterDependencies(configuration); },
    app =>
    {
        // DB Migration
        using IServiceScope scope = app.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        app.AddProjectEndpoints();
        app.AddTodoEndpoints();
    });
