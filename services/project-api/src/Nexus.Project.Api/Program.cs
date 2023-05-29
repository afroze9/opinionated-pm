using Microsoft.EntityFrameworkCore;
using Nexus.ProjectAPI.Data;
using Nexus.ProjectAPI.Endpoints;
using Nexus.ProjectAPI.Extensions;

CoreWebApplicationBuilder.BuildConfigureAndRun(
    args,
    configureDefaultMiddleware: true,
    preConfiguration: null,
    registerServices: (services, configuration, _) => { services.RegisterDependencies(configuration); },
    configureMiddleware: app =>
    {
        // DB Migration
        using IServiceScope scope = app.Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();

        app.AddProjectEndpoints();
        app.AddTodoEndpoints();
    });