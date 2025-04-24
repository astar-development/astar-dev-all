using System.Diagnostics.CodeAnalysis;
using AStar.Dev.Database.Updater.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater;

[ExcludeFromCodeCoverage]
public sealed class DeleteMarkedFiles(
    FilesService                       filesService,
    TimeProvider                       timeProvider,
    ILogger<UpdateDatabaseForAllFiles> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DeleteMarkedFiles started at: {RunTime} (Local Time)", timeProvider.GetLocalNow());

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (UpdateDatabaseForAllFiles.GlobalUpdateIsRunning)
                {
                    logger.LogInformation("Waiting for an hour before restarting at: {RunTime} (Local Time)",
                                          timeProvider.GetLocalNow().AddHours(1));

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                    return;
                }

                await filesService.DeleteFilesMarkedForHardDeletionAsync(stoppingToken);
                await filesService.DeleteFilesMarkedForSoftDeletionAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in AStar.Update.Database.WorkerService: {ErrorMessage}",
                                ex.Message);
            }

            logger.LogInformation("Waiting for an hour before restarting at: {RunTime} (Local Time)",
                                  timeProvider.GetLocalNow().AddHours(1));

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
