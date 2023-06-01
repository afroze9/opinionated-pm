using FluentValidation;
using OpenTelemetry.Resources;
using PeopleAPI.Abstractions;
using PeopleAPI.Data;
using PeopleAPI.Data.Repositories;
using PeopleAPI.Mapping;
using PeopleAPI.Services;
using PeopleAPI.Telemetry;
using Steeltoe.Common.Http.Discovery;

namespace PeopleAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Internal Services
        services.AddSingleton<IPeopleInstrumentation, PeopleInstrumentation>();
        
        // Custom Meter for Metrics
        services.AddOpenTelemetry()
            .ConfigureResource(c =>
            {
                c.AddService("people-api");
            })
            .WithMetrics(builder =>
            {
                builder.AddMeter(PeopleInstrumentation.MeterName);
            });
        
        services.AddScoped<IPeopleService, PeopleService>();

        // Libraries
        services.AddAutoMapper(typeof(PeopleProfile));
        services.AddValidatorsFromAssemblyContaining(typeof(Program));

        // Persistence
        services.AddCorePersistence<ApplicationDbContext>(configuration);
        services.AddScoped<PeopleRepository>();
        services.AddScoped<UnitOfWork>();
    }
}