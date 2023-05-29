using Nexus.HealthChecksDashboard.Extensions;

CoreWebApplicationBuilder.BuildConfigureAndRun(args,
    configureDefaultMiddleware:false,
    preConfiguration:null,
    (services, configuration, _) =>
    {
        services.RegisterDependencies(configuration);
    },
    configureMiddleware: app =>
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
    });
