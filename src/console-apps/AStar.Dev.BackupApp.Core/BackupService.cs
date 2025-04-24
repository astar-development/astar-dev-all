using System.IO.Abstractions;
using System.IO.Compression;
using AStar.Dev.BackupApp.Core.Models;
using Microsoft.Extensions.Options;

namespace AStar.Dev.BackupApp.Core;

public sealed class BackupService(IOptions<BackupSettings> backupConfiguration, IFileSystem fileSystem) : IBackupService
{
    private readonly BackupSettings backupConfiguration = backupConfiguration.Value;

    public void RunBackup()
    {
        string[] pathsToBackup       = backupConfiguration.PathsToBackup;
        string   defaultPathToBackup = backupConfiguration.BackupToPath;

        Console.WriteLine($"Starting back using the defaultPathToBackup: {defaultPathToBackup}");

        foreach (string? pathToBackup in pathsToBackup)
        {
            int      rootDirectoryIndex = pathToBackup.LastIndexOf('\\');
            string   rootDirectoryName  = pathToBackup[..rootDirectoryIndex];
            string[] directories        = fileSystem.Directory.GetDirectories(pathToBackup, "*", SearchOption.TopDirectoryOnly);

            foreach (string? directory in directories)
            {
                if (directory.Contains("AStar.Dev.BackupApp", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string directoryNameWithoutPath = $"{defaultPathToBackup}\\{directory.Replace(rootDirectoryName, string.Empty)}.zip".Replace(@"\\",
                                                                                                                                             "\\");

                Console.WriteLine($"Backing up: {directory} to {directoryNameWithoutPath}");

                if (fileSystem.File.Exists(directoryNameWithoutPath))
                {
                    fileSystem.File.Delete(directoryNameWithoutPath);
                }

                int    backupDirectoryIndex = directoryNameWithoutPath.LastIndexOf('\\');
                string backupDirectoryName  = directoryNameWithoutPath[..backupDirectoryIndex];

                if (!fileSystem.Directory.Exists(backupDirectoryName))
                {
                    _ = fileSystem.Directory.CreateDirectory(backupDirectoryName);
                }

                ZipFile.CreateFromDirectory(directory, directoryNameWithoutPath);

                Console.WriteLine($"Backed up: {directory} to {directoryNameWithoutPath}");
            }
        }
    }
}
