using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.ProjectAPI.Entities;

namespace Nexus.ProjectAPI.Data.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    /// <summary>
    ///     Configures the entity type and its relationships.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        // Configures the maximum length and requirement of the todoitem title.
        builder.Property(c => c.Title)
            .HasMaxLength(255)
            .IsRequired();

        // Configures the table name for the company entity.
        builder.ToTable("TodoItem");
    }
}