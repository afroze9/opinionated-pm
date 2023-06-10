# Adding a new Service

From the root of the application, run the following command:

```powershell
nexus add service "service_name"
```

## Entities
To add a new entity, create a new class in the `Entities` folder and have it extend the `AuditableEntityBase` class:
```csharp
public class Company : AuditableEntityBase
{
    public Company(string name)
    {
        Name = name;
    }
    
    public string Name { get; private set; }
    
    public virtual List<Tag> Tags { get; set; } = new ();
}
```

The `AuditableEntityBase` class contains fields used to manage audit information including when the entity was 
created/modifed and by which user. It also contains an Identifier field.

## Database Access
### Entity Configuration
For each added entity, you might want to create a configuration file that tells entity framework how to setup the 
fields and the table. This can be done by adding a new Configuration class in the `Data/Configuration` folder:
```csharp
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Configures the maximum length and requirement of the company name.
        builder.Property(c => c.Name)
            .HasMaxLength(255)
            .IsRequired();

        // Configures the table name for the company entity.
        builder.ToTable("Company");

        // Configures a many-to-many relationship between the company and tag entities.
        builder
            .HasMany<Tag>(c => c.Tags)
            .WithMany(t => t.Companies);
    }
}
```

### ApplicationDbContext
The entities will need to be added to the `ApplicationDbContext` class:
```csharp
public class ApplicationDbContext : AuditableDbContext
{
    // Rest of the code

    public DbSet<Company> Companies => Set<Company>();

    // Rest of the code
}
```

### Repositories
Repositories provide a standard way to fetch data from the Database. The EfCustomRepository provides a set of 
methods for this. It can be extended to allow for custom functionality on top of the provided one:
```csharp
public class CompanyRepository : EfCustomRepository<Company>
{
    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Company>> AllCompaniesWithTagsAsync()
    {
        return await DbSet.Include(x => x.Tags).ToListAsync();
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Company?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
```

### UnitOfWork
In case you need transactional support, you can also add the Repository to the `UnitOfWork`:
```csharp
public class UnitOfWork : UnitOfWorkBase
{
    public UnitOfWork(ApplicationDbContext context,
        CompanyRepository companyRepository)
        : base(context)
    {
        Companies = companyRepository;
    }

    public CompanyRepository Companies { get; }
}
```

### Migrations
To add migrations, run the following commands from the csproj directory:
```powershell
dotnet ef migrations add "Init" --output-dir .\Data\Migrations
dotnet ef database update
```
The first command adds a new migration called "Init" and stores the state of the `ApplicationDbContext` in the 
`Data/Migrations` folder.

The second commands applies the migrations to the database.

## Services
* Services contain business logic for the service
* The interfaces for the services are added to the `Abstractions` folder
* The implementations in the `Services` folder
* Services can communicate with the database using the repositories and UnitOfWork classes
* Services return DTOs which are stored in the `DTO` folder

## Api Controllers and Request/Response Models
* Controllers consume the entity services and provide REST functionality
* The endpoints accept Requests. These are special kinds of DTOs and are stored in the `Model` folder. They follow the
name convention `<Entity><Action?>RequestModel`. e.g. `CompanyCreateRequestModel`.
* The endpoints return Responses. These are special kinds of DTOs and are stored in the `Model` folder. They follow the
  name convention `<Entity><Action?>ResponseModel`. e.g. `CompanyResponseModel`.

## Request Model Validation
The services are setup to use FluentValidation to ensure the requests are valid. The validators extend the 
`AbstractValidator<T>` class provided by FluentValidation:
```csharp
public class CompanyRequestModelValidator : AbstractValidator<CompanyRequestModel>
{
    public CompanyRequestModelValidator()
    {
        RuleFor(c => c.Name)
            .NotNull()
            .MinimumLength(5)
            .MaximumLength(255);
    }
}
```
The validators are stored in the `Model/Validation` folder.

## Mapping Profiles
To translate between Entities, DTOs, and models, the service is setup to use Automapper. Mapping profiles are stored in
the `Mapping` folder:
```csharp
public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanySummaryDto>();
        CreateMap<Company, CompanyDto>();
        CreateMap<CompanySummaryDto, Company>();
        CreateMap<CompanyDto, CompanyResponseModel>();
    }
}
```

## Customizing Bootstrapper
Read the guide on setting up the framework [here](../libraries/src/Nexus.Framework.Web/README.md)

## Custom Telemetry
TODO