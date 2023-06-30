using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <summary>
    ///     Configures the entity type and its relationships.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Configures the maximum length and requirement of the project name.
        builder.Property(c => c.Name)
            .HasMaxLength(255)
            .IsRequired();

        // Configures the table name for the company entity.
        builder.ToTable("Project");

        // Configures a many-to-many relationship between the company and tag entities.
        builder.HasMany<TodoItem>(p => p.TodoItems);
    }
}