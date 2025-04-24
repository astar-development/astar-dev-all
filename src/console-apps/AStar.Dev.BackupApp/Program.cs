using System.IO.Abstractions;
using AStar.Dev.BackupApp.Core;
using AStar.Dev.BackupApp.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AStar.Dev.BackupApp;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();

        IBackupService backupService = host.Services.GetRequiredService<IBackupService>();
        backupService.RunBackup();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) => _ = config.AddJsonFile("appsettings.json", false, true))
            .ConfigureServices((context, services) =>
                               {
                                   _ = services.AddTransient<IBackupService, BackupService>();
                                   _ = services.AddScoped<IFileSystem, FileSystem>();

                                   _ = services.AddOptions<BackupSettings>()
                                               .BindConfiguration(BackupConfiguration.BackupSettingsName)
                                               .ValidateDataAnnotations()
                                               .ValidateOnStart();
                               });
}
