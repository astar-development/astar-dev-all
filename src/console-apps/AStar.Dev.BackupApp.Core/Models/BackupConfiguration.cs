using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.BackupApp.Core.Models;

public sealed class BackupConfiguration
{
    public static string BackupSettingsName => "backupSettings";

    [Required]
    public BackupSettings BackupSettings { get; set; } = new();
}
