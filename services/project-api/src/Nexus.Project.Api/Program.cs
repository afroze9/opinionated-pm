using Microsoft.EntityFrameworkCore;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Endpoints;
using Nexus.ProjectAPI.Extensions;

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
