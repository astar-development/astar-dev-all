using System.IO.Abstractions;
using System.Text.Json;
using AStar.Dev.Infrastructure.FileClassificationsDb.Models;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Technical.Debt.Reporting;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using FileAccessDetail = AStar.Dev.Infrastructure.FilesDb.Models.FileAccessDetail;
using FileDetail = AStar.Dev.Infrastructure.FilesDb.Models.FileDetail;

namespace AStar.Dev.Database.Updater.Services;

[Refactor(5, 10, "Need to refactor to use the existing APIs - extending them if they do not support the requirements")]
public sealed class FilesService(FilesContext context, IFileSystem fileSystem, ILogger<FilesService> logger)
{
    private readonly List<FileDetailClassification> cachedFileClassifications = [];
    private readonly List<ClassificationMapping>    classificationMappings    = [];

    public async Task DeleteFilesMarkedForSoftDeletionAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting removal of files marked for soft deletion");

        List<FileAccessDetail> fileAccessDetails = await context.FileAccessDetails.AsNoTracking().Where(fileAccess => fileAccess.SoftDeletePending)
                                                                .ToListAsync(cancellationToken);

        logger.LogInformation("There are {Files} files marked for soft deletion", fileAccessDetails.Count);

        foreach (FileAccessDetail fileAccessDetail in fileAccessDetails)
        {
            FileDetail fileDetail = await context.Files.AsNoTracking().SingleAsync(file => file.FileAccessDetail.Id == fileAccessDetail.Id,
                                                                                   cancellationToken);

            logger.LogInformation("Soft-deleting file: {FileName} from {DirectoryName}", fileDetail.FileName,
                                  fileDetail.DirectoryName);

            DeleteFileIfItExists(fileDetail);

            fileAccessDetail.SoftDeletePending = false;
            fileAccessDetail.SoftDeleted       = true;
        }

        await SaveChangesSafely(cancellationToken);
        logger.LogInformation("Completed removal of files marked for soft deletion");
    }

    public async Task DeleteFilesMarkedForHardDeletionAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting removal of files marked for hard deletion");

        // var fileAccessDetails = await context.FileAccessDetails.Where(fileAccess => fileAccess.HardDeletePending)
        //                                      .ToListAsync(cancellationToken);
        //
        // logger.LogInformation("There are {Files} files marked for hard deletion", fileAccessDetails.Count);
        //
        // foreach(var fileAccessDetail in fileAccessDetails)
        // {
        //     var fileDetail = await context.Files.SingleAsync(file => file.FileAccessDetail.ClassificationId == fileAccessDetail.ClassificationId,
        //                                                      cancellationToken);
        //
        //     logger.LogInformation("Hard-deleting file: {FileName} from {DirectoryName}", fileDetail.FileName,
        //                           fileDetail.DirectoryName);
        //
        //     DeleteFileIfItExists(fileSystem, fileDetail);
        //
        //     _ = context.Files.Remove(fileDetail);
        //     _ = context.FileAccessDetails.Remove(fileAccessDetail);
        // }

        await SaveChangesSafely(cancellationToken);
        logger.LogInformation("Completed removal of files marked for hard deletion");
    }

    public IEnumerable<string> GetFilesFromDirectory(string dir, string searchPattern = "*.*", bool recursive = true)
    {
        logger.LogInformation("Getting files in {Directory}", dir);

        string[] files = fileSystem.Directory.GetFiles(dir, searchPattern,
                                                       new EnumerationOptions { RecurseSubdirectories = recursive, IgnoreInaccessible = true, }
                                                      );

        logger.LogInformation("Got files in {Directory}", dir);

        return files;
    }

    public async Task ProcessNewFiles(IEnumerable<string> files, CancellationToken stoppingToken)
    {
        // classificationMappings = await AddClassificationMappingsAsync(stoppingToken);
        //
        // foreach (ClassificationMapping classificationMapping in classificationMappings)
        // {
        //     if (await context.FileClassifications.FirstOrDefaultAsync(c => c.Name == classificationMapping.DatabaseMapping, stoppingToken) is not null)
        //     {
        //         continue;
        //     }
        //
        //     await context.FileClassifications.AddAsync(new () { Name = classificationMapping.DatabaseMapping, }, stoppingToken);
        //     await context.SaveChangesAsync(stoppingToken);
        // }
        //
        // List<FileClassification> fileClassifications = await context.FileClassifications.ToListAsync(stoppingToken);
        //
        // foreach (FileClassification fileClassification in fileClassifications)
        // {
        //     cachedFileClassifications.Add(fileClassification);
        // }

        IQueryable<string> filesInDb        = context.Files.AsNoTracking().Select(file => Path.Combine(file.DirectoryName, file.FileName));
        var                notInTheDatabase = files.Except(filesInDb).ToList();

        await ProcessFilesNotInTheDatabase(notInTheDatabase, stoppingToken);

        await SaveChangesSafely(stoppingToken);
    }

    private async Task<List<ClassificationMapping>> AddClassificationMappingsAsync(CancellationToken stoppingToken)
    {
        string fileContent = await fileSystem.File.ReadAllTextAsync(@"c:\temp\image-mappings.json", stoppingToken);

        List<ClassificationMapping>? mappings = JsonSerializer.Deserialize<List<ClassificationMapping>>(fileContent);

        return mappings ?? [];
    }

    private async Task ProcessFilesNotInTheDatabase(List<string> notInTheDatabase, CancellationToken stoppingToken)
    {
        var counter = 0;

        foreach (string file in notInTheDatabase)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                await SaveChangesSafely(stoppingToken);

                break;
            }

            int    lastIndexOf   = file.LastIndexOf('\\');
            string directoryName = file[..lastIndexOf];
            string fileName      = file[++lastIndexOf..];

            if (await context.Files.AsNoTracking().AnyAsync(fileDetail => fileDetail.FileName == fileName && fileDetail.DirectoryName == directoryName,
                                                            stoppingToken))
            {
                continue;
            }

            await AddNewFileAsync(file, context, stoppingToken);
            counter++;

            if (counter < 100)
            {
                continue;
            }

            counter = 0;
            await SaveChangesSafely(stoppingToken);
            int count = context.Files.Count();

            logger.LogInformation("Updated the database. File {FileName} has been added to the database. There are now {FileCount} files in the database", file, count);
        }
    }

    public async Task ProcessMovedFiles(IEnumerable<string> files, string[] directories, CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting update of files that have moved");
        string[] filesAsArray = files.ToArray();

        foreach (string directory in directories)
        {
            foreach (string file in filesAsArray.Where(file => file.StartsWith(directory)))
            {
                int    lastIndexOf   = file.LastIndexOf('\\');
                string directoryName = file[..lastIndexOf];
                string fileName      = file[++lastIndexOf..];

                FileDetail? movedFile = await context.Files.FirstOrDefaultAsync(
                                                                                f => f.DirectoryName.StartsWith(directory) && f.DirectoryName != directoryName &&
                                                                                     f.FileName                                               == fileName, stoppingToken);

                if (movedFile != null)
                {
                    await UpdateExistingFile(directoryName, fileName, movedFile, stoppingToken);
                }
            }
        }

        await SaveChangesSafely(stoppingToken);
    }

    public async Task RemoveFilesFromDbThatDoNotExistAnyMore(IEnumerable<string> files, CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting removal of files deleted from disc outside of the UI.");

        IQueryable<string> filesInDb = context.Files
                                              .Include(x => x.FileAccessDetail)
                                              .Where(file => !file.FileAccessDetail.SoftDeleted && !file.FileAccessDetail.SoftDeletePending &&
                                                             !file.FileAccessDetail.HardDeletePending)
                                              .Select(file => Path.Combine(file.DirectoryName, file.FileName));

        List<string> notOnDisc = await filesInDb.Except(files).ToListAsync(stoppingToken);

        foreach (string file in notOnDisc)
        {
            int    lastIndexOf   = file.LastIndexOf('\\');
            string directoryName = file[..lastIndexOf];
            string fileName      = file[++lastIndexOf..];

            FileDetail fileDetail = await context.Files.SingleAsync(f => f.DirectoryName == directoryName && f.FileName == fileName,
                                                                    stoppingToken);

            int fileCount             = await context.Files.CountAsync(stoppingToken);
            int fileAccessDetailCount = await context.FileAccessDetails.CountAsync(stoppingToken);
            _ = context.Files.Remove(fileDetail);
            await SaveChangesSafely(stoppingToken);
            int fileCountAfter             = await context.Files.CountAsync(stoppingToken);
            int fileAccessDetailCountAfter = await context.FileAccessDetails.CountAsync(stoppingToken);

            logger.LogInformation(
                                  "File Count before: {FileCount} File Access Detail Count before: {FileAccessDetailCount}, File Count after: {FileCountAfter} File Access Detail Count after: {FileAccessDetailCountAfter}",
                                  fileCount, fileAccessDetailCount, fileCountAfter, fileAccessDetailCountAfter);
        }

        await SaveChangesSafely(stoppingToken);
    }

    private async Task UpdateExistingFile(string            directoryName,
                                          string            fileName,
                                          FileDetail        fileFromDatabase,
                                          CancellationToken stoppingToken)
    {
        foreach (FileDetail file in context.Files.Where(file => file.FileName == fileName))
        {
            _ = context.Files.Remove(file);
        }

        await SaveChangesSafely(stoppingToken);

        var updatedFile = new FileDetail
                          {
                              DirectoryName    = directoryName,
                              Height           = fileFromDatabase.Height,
                              Width            = fileFromDatabase.Width,
                              FileName         = fileName,
                              FileSize         = fileFromDatabase.FileSize,
                              FileAccessDetail = new() { SoftDeleted = false, SoftDeletePending = false, DetailsLastUpdated = DateTime.UtcNow , },
                          };

        _ = await context.Files.AddAsync(updatedFile, stoppingToken);

        logger.LogInformation(
                              "File: {FileName} ({OriginalLocation}) appears to have moved since being added to the dB - previous location: {DirectoryName}",
                              fileName, directoryName, fileFromDatabase.DirectoryName);
    }

    private void DeleteFileIfItExists(FileDetail fileDetail)
    {
        try
        {
            if (fileSystem.File.Exists(fileDetail.FullNameWithPath))
            {
                fileSystem.File.Delete(fileDetail.FullNameWithPath);
            }
        }
        catch (DirectoryNotFoundException ex)
        {
            logger.LogWarning(ex, "Directory not found: {FullNameWithPath}", fileDetail.FullNameWithPath);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogWarning(ex, "File not found: {FullNameWithPath}", fileDetail.FullNameWithPath);
        }
    }

    private async Task AddNewFileAsync( string file, FilesContext filesContext, CancellationToken stoppingToken)
    {
        List<FileDetailClassification> fileDetailClassifications = await GetFileDetailClassificationsFromFilenameAsync(file, filesContext, stoppingToken);

        try
        {
            IFileInfo fileInfo   = fileSystem.FileInfo.New(file);

            var       fileDetail = new FileDetail { FileName = fileInfo.Name, DirectoryName = fileInfo.DirectoryName!, FileSize = fileInfo.Length, IsImage = fileInfo.IsImage(), };

            if (fileDetail.IsImage)
            {
                var image = SKImage.FromEncodedData(file);

                if (image is not null)
                {
                    fileDetail.Height = image.Height;
                    fileDetail.Width  = image.Width;
                }
            }

            var fileAccessDetail = new FileAccessDetail { SoftDeleted = false, SoftDeletePending = false, DetailsLastUpdated = DateTime.UtcNow, };

            fileDetail.FileAccessDetail = fileAccessDetail;
            _                           = context.Files.Add(fileDetail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving file '{File}' details", file);
        }
    }

    private async Task<List<FileDetailClassification>> GetFileDetailClassificationsFromFilenameAsync(string fileName, FilesContext filesContext, CancellationToken stoppingToken)
    {
        List<FileDetailClassification> fileDetailClassifications = [];

        foreach (ClassificationMapping classificationMapping in classificationMappings.Where(classificationMapping => fileName.Contains(classificationMapping.FileNameContains)))
        {
            int  classificationId = await GetOrCreateFileClassificationAsync(filesContext, classificationMapping.DatabaseMapping, stoppingToken);

            if (fileDetailClassifications.Any(f => f.Id == classificationId))
            {
                continue;
            }

            fileDetailClassifications.Add(new () {  Id = classificationId, });
        }

        return fileDetailClassifications;
    }

    [Refactor(2, 4, "Look at replacing with the Find method as we have the same context")]
    private async Task<int> GetOrCreateFileClassificationAsync(FilesContext filesContext, string classificationName, CancellationToken stoppingToken)
    {
        await Task.Delay(1, stoppingToken);
        FileDetailClassification? local = cachedFileClassifications.SingleOrDefault(f => f.Name == classificationName);

        if (local is not null)
        {
            return local.Id;
        }

        // FileDetailClassification? fileClassification = await filesContext.FileDetailClassification.SingleOrDefaultAsync(x => x.Name.Contains(classificationName), stoppingToken);
        //
        // if (fileClassification is not null)
        // {
        //     cachedFileClassifications.Add(fileClassification);
        //
        //     return fileClassification.Id;
        // }
        //
        // await filesContext.FileClassifications.AddAsync(new () { Name = classificationName, }, stoppingToken);
        // await filesContext.SaveChangesAsync(stoppingToken);
        // local = (await filesContext.FileClassifications.SingleOrDefaultAsync(x => x.Name.Contains(classificationName), stoppingToken))!;

        // cachedFileClassifications.Add(local);

        return local?.Id ?? -1;
    }

    private async Task SaveChangesSafely(CancellationToken stoppingToken)
    {
        try
        {
            await context.SaveChangesAsync(stoppingToken);
        }
        catch (DbUpdateException ex)
        {
            if (ex.Message.Contains("The timeout period elapsed"))
            {
                try
                {
                    await SaveChangesSafely(stoppingToken);
                }
                catch
                {
                    //
                }
            }
            else             if (!ex.Message.StartsWith("The database operation was expected to affect"))
            {
                logger.LogError(ex, "Error: {Error} occurred whilst saving changes - probably 'no records affected'",
                                ex.Message);
            }
        }
    }
}
