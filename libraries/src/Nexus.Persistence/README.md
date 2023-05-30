# How to Use

## AuditableDbContext

To enable auditing for entities, create an `ApplicationDbContext` and extend `AuditableDbContext`:
```csharp
public class ApplicationDbContext : AuditableDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options, auditableEntitySaveChangesInterceptor)
    {
    }
    
    public DbSet<Company> Companies => Set<Company>();
}
```

## IEntityTypeConfiguration
To apply any Entity Type Configurations, override the `OnModelCreating` method:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
```

## Configuration

Ensure the following settings exist in appsettings.json / Environment variables / Consul KV:

```json
{
  "Postgres": {
    "Client": {
      "Host": "<db-host>",
      "Port": 5438,
      "Username": "<db-username>",
      "Password": "<db-password>",
      "Database": "<db-name>"
    }
  }
}
```

## Usage

While registering services, call:

```
services.AddCorePersistence<ApplicationDbContext>(configuration);
```
