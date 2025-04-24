using AStar.Dev.Infrastructure.FileClassificationsDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FileClassificationsDb.Configurations;

/// <summary>
/// </summary>
public class FileDetailClassificationsConfiguration : IEntityTypeConfiguration<FileDetailClassification>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FileDetailClassification> builder)
    {
        _ = builder
           .ToTable(nameof(FileDetailClassification), Constants.SchemaName)
           .HasKey(fileDetailClassification =>  fileDetailClassification.Id);

        _ = builder.Property(fileDetailClassification => fileDetailClassification.Name).HasMaxLength(150);
    }
}
