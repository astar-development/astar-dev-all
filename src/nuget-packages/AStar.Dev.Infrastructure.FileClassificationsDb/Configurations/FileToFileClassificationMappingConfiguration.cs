using AStar.Dev.Infrastructure.FileClassificationsDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FileClassificationsDb.Configurations;

/// <summary>
/// </summary>
public class FileToFileClassificationMappingConfiguration : IEntityTypeConfiguration<FileToFileClassificationMapping>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FileToFileClassificationMapping> builder)
    {
        _ = builder
           .ToTable(nameof(FileToFileClassificationMapping), Constants.SchemaName)
           .HasKey(fileToFileClassificationMapping => new { fileToFileClassificationMapping.FileId, fileToFileClassificationMapping.ClassificationId, });
    }
}
