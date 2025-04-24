using System.Web;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Services;

/// <summary>
/// </summary>
/// <param name="filesApiClient"></param>
/// <param name="logger"></param>
public class DatabaseUpdateService(FilesApiClient filesApiClient, ILogger<FilesService> logger)
{
    public async Task ProcessNewFiles(string directory, CancellationToken stoppingToken)
    {
        // Get the token somehow...

        directory = HttpUtility.UrlDecode(directory);
        string[] directories = (await filesApiClient.GetDirectoriesAsync(directory, stoppingToken)).ToArray();
        logger.LogDebug("Found {DirectoryCount} directories in Directory: {DirectoryName}", directories.Length, directory);

        await NewMethod( directories, stoppingToken);
    }

    private async Task NewMethod( string[] directories, CancellationToken stoppingToken)
    {
        foreach (string directoryName in directories)
        {
            string directoryNameDecoded = HttpUtility.UrlDecode(directoryName);
            directories         = (await filesApiClient.GetDirectoriesAsync(directoryNameDecoded, stoppingToken)).ToArray();
            logger.LogDebug("Found {DirectoryCount} directories in Directory: {DirectoryName}", directories.Length, directoryNameDecoded);

            await NewMethod( directories, stoppingToken);
        }
    }
}
