using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.BackupApp.Core.Models;

public sealed class BackupSettings
{
    [Required]
    public string[] PathsToBackup { get; set; } = [];

    [Required]
    public string BackupToPath { get; set; } = string.Empty;
}
