using System.Diagnostics.CodeAnalysis;
using AStar.Dev.Database.Updater.Models;
using AStar.Dev.Database.Updater.Services;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater;

[ExcludeFromCodeCoverage]
public sealed class UpdateDatabaseForAllFiles(
    FilesService                       filesService,
    DatabaseUpdateService              databaseUpdateService,
    IOptions<ApiConfiguration>         directories,
    ILogger<UpdateDatabaseForAllFiles> logger)
    : BackgroundService
{
    public static bool GlobalUpdateIsRunning { get; private set; }

    [Refactor(5, 10, "Refactor to use the new DatabaseUpdateService when available")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("Processing new files...ExecuteAsync method");
        await databaseUpdateService.ProcessNewFiles("Pictures", stoppingToken);

        // const string endTime = "5:00 AM";
        // logger.LogInformation("UpdateDatabaseForAllFiles started at: {RunTime}", DateTime.Now);
        // TimeSpan initialRunDelay = TimeDelay.CalculateDelayToNextRun(endTime);
        //
        // logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database.", initialRunDelay);
        //
        // // await Task.Delay(initialRunDelay, stoppingToken);
        //
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     try
        //     {
        //         GlobalUpdateIsRunning = true;
        //         List<string> files = GetFiles();
        //
        //         await filesService.ProcessNewFiles(files, stoppingToken);
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex, "Error occurred in AStar.Update.Database.WorkerService: {ErrorMessage}",
        //                         ex.Message);
        //     }
        //
        //     TimeSpan delayToNextRun = TimeDelay.CalculateDelayToNextRun(endTime);
        //
        //     logger.LogInformation("Waiting for: {DelayToNextRun} hours before updating the full database again.",
        //                           delayToNextRun);
        //
        //     GlobalUpdateIsRunning = false;
        //     await Task.Delay(delayToNextRun, stoppingToken);
        // }
    }

    private List<string> GetFiles()
    {
        var files = new List<string>();

        foreach (string dir in directories.Value.Directories)
        {
            files.AddRange(filesService.GetFilesFromDirectory(dir));
        }

        return files;
    }
}
