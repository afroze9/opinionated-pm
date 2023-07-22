using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Data.Configuration;

[ExcludeFromCodeCoverage]
public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.Property(c => c.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.IdentityId)
            .IsRequired();
        
        builder.ToTable("Person")
            .HasIndex(p => p.IdentityId)
            .IsUnique();
    }
}