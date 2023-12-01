using System.Data.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Nexus.CompanyAPI.Data;
using Testcontainers.PostgreSql;

namespace Nexus.CompanyAPI.IntegrationTests;

[ExcludeFromCodeCoverage]
public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : Program
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithUsername("developer")
        .WithPassword("dev123")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            ServiceDescriptor? dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(_ =>
            {
                NpgsqlConnection connection = new (_dbContainer.GetConnectionString());
                connection.Open();
                return connection;
            });

            services.AddDbContext<ApplicationDbContext>((container, options) =>
            {
                DbConnection connection = container.GetRequiredService<DbConnection>();
                options.UseNpgsql(connection);
            });

            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new TestJwtSecurityTokenHandler());
            });
        });
        
        builder.UseEnvironment("Development");
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }
        
    public Task DisposeAsync()
    {
        return _dbContainer.DisposeAsync().AsTask();
    }
}