using System.Diagnostics.CodeAnalysis;
using AStar.Dev.Database.Updater.Models;
using AStar.Dev.Database.Updater.Services;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Directory = AStar.Dev.Database.Updater.Models.Directory;

namespace AStar.Dev.Database.Updater;

[ExcludeFromCodeCoverage]
public sealed class MoveFiles(
    FilesContext                       context,
    IOptions<DirectoryChanges>         directories,
    FilesApiClient                     filesApiClient,
    ILogger<UpdateDatabaseForAllFiles> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MoveFiles started at: {RunTime}", DateTime.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            const string endTime = "3:00 AM";

            try
            {
                logger.LogInformation("MoveFiles started at: {RunTime}", DateTime.Now);
                TimeSpan intialRunDelay = TimeDelay.CalculateDelayToNextRun(endTime);

                logger.LogInformation(
                                      "MoveFiles Waiting for: {DelayToNextRun} hours before updating the marked to move files .",
                                      intialRunDelay);

                await Task.Delay(intialRunDelay, stoppingToken);

                await MoveFilesToTheirNewHomeAsync();
                TimeSpan delayToNextRun = TimeDelay.CalculateDelayToNextRun(endTime);

                logger.LogInformation(
                                      "MoveFiles Waiting for: {DelayToNextRun} hours before updating the marked to move files again.",
                                      delayToNextRun);

                await Task.Delay(delayToNextRun, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in AStar.Update.Database.WorkerService: {ErrorMessage}",
                                ex.Message);
            }
        }
    }

    private async Task MoveFilesToTheirNewHomeAsync()
    {
        if (UpdateDatabaseForAllFiles.GlobalUpdateIsRunning)
        {
            return;
        }

        foreach (Directory directory in directories.Value.Directories)
        {
            logger.LogInformation("Getting the files from the database that contain the {DirectoryName}.", directory);

            IQueryable<FileDetail> filesToMove = context.Files.Include(x => x.FileAccessDetail)
                                                        .Where(file => !file.FileAccessDetail.SoftDeleted && file.DirectoryName.Contains(directory.Old));

            foreach (FileDetail fileToMove in filesToMove)
            {
                Files.Api.Client.SDK.Models.FileDetail file             = await filesApiClient.GetFileDetail(fileToMove.Id);
                string                                 fullNameWithPath = file.FullNameWithPath;
                string                                 newLocation      = file.DirectoryName.Replace(directory.Old, directory.New);

                try
                {
                    _ = await filesApiClient.UpdateFileAsync(new() { OldDirectoryName = file.DirectoryName, NewDirectoryName = newLocation, FileName = file.FileName, });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error: {Error} occurred whilst updating the directory for {FileName}",
                                    fullNameWithPath, ex.Message);
                }
            }
        }
    }
}
