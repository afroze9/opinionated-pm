using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexus.PeopleAPI.Entities;

namespace Nexus.PeopleAPI.Data.Configuration;

[ExcludeFromCodeCoverage]
public class SyncStatusConfiguration : IEntityTypeConfiguration<SyncStatus>
{
    public void Configure(EntityTypeBuilder<SyncStatus> builder)
    {
        builder.Property(s => s.JobName)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.ToTable("SyncStatus");
    }
}