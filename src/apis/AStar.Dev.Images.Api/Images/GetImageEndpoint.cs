using System.IO.Abstractions;
using Asp.Versioning.Builder;
using AStar.Dev.Images.Api.Extensions;
using AStar.Dev.Images.Api.Models;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Technical.Debt.Reporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace AStar.Dev.Images.Api.Images;

public sealed class GetImageEndpoint(WebApplication app) : IEndpoint
{
    private const int MaximumHeightAndWidthForResizedImage = 850;

    private static readonly Dictionary<string, SKEncodedImageFormat> SkiaSharpImageFormatMapping = new(StringComparer.InvariantCultureIgnoreCase)
                                                                                                   {
                                                                                                       { ".png", SKEncodedImageFormat.Png },
                                                                                                       { ".jpg", SKEncodedImageFormat.Jpeg },
                                                                                                       { ".jpeg", SKEncodedImageFormat.Jpeg },
                                                                                                       { ".jpe", SKEncodedImageFormat.Jpeg },
                                                                                                   };

    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ImageGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ImageEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/", async ([FromQuery]    string                    imagePath,
                               [FromServices] ILogger<GetImageEndpoint> logger,
                               [FromQuery]    bool                      resizeImage         = true,
                               [FromQuery]    int                       maximumSizeInPixels = 850,
                               [FromServices] IFileSystem               fileSystem          = null!,
                               [FromServices] FilesContext              context             = null!,
                               CancellationToken                        cancellationToken   = default)  =>
                            await Handle(imagePath, resizeImage, maximumSizeInPixels, fileSystem, context, logger, cancellationToken))
           .AddBasicProduces<FileStream>()
           .WithDescription("Get the file Image matching the specified criteria.")
           .WithSummary("Get the file Image")
           .RequireAuthorization()
           .WithTags(EndpointConstants.ImageTag);
    }

    [Refactor(5, 2, "Too long")]
    private static async Task<IResult> Handle(string                    imagePath,
                                              bool                      resizeImage,
                                              int                       maximumSizeInPixels,
                                              IFileSystem               fileSystem,
                                              FilesContext              context,
                                              ILogger<GetImageEndpoint> logger,
                                              CancellationToken         cancellationToken)
    {
        try
        {
            if (!imagePath.IsImage())
            {
                logger.LogDebug("{FileName} is not an image, exiting...", imagePath);

                return Results.BadRequest("Unsupported file type.");
            }

            imagePath = imagePath.Replace('/', '\\');

            logger.LogDebug(
                            "Starting retrieval for the {ImagePath} resizeImage: {ResizeImage}, maximumSizeInPixels: {MaximumSizeInPixels}",
                            imagePath, resizeImage, maximumSizeInPixels);

            int         index      = imagePath.LastIndexOf('\\');
            string      directory  = imagePath[..index];
            string      filename   = imagePath[(index + 1)..];
            FileDetail? fileInfoJb = ReadDb(context, directory, filename);

            if (fileInfoJb is not null)
            {
                fileInfoJb.FileAccessDetail.LastViewed = DateTime.UtcNow;

                try
                {
                    logger.LogDebug("Updating Last Viewed for: {FileName}", filename);
                    _ = await context.SaveChangesAsync(cancellationToken);
                }
                catch
                {
                    // Any error here is not important.
                }
            }

            if (fileInfoJb is null || !fileSystem.File.Exists(imagePath))
            {
                logger.LogDebug("{FileName} was not found, exiting...", filename);

                return Results.NotFound();
            }

            if (resizeImage)
            {
                logger.LogDebug("Resized Image being created for: {FileName}", filename);
                int          indexOfExtension = imagePath.LastIndexOf('.');
                MemoryStream outputStream     = ResizeImage(imagePath, fileInfoJb.Width, fileInfoJb.Height, imagePath[indexOfExtension..], maximumSizeInPixels);

                return Results.File(outputStream, "image/jpeg");
            }

            logger.LogDebug("Resized Image wasn't created for: {FileName}", filename);
            string fileExtension = filename.Split('.').Last();

            return fileExtension is "jpg" or "jpeg" ? Results.File(imagePath, "image/jpeg") : Results.File(imagePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading image: {ImageLoadError}", ex.GetBaseException().Message);

            throw;
        }
    }

    private static MemoryStream ResizeImage(   string imagePath, int width, int height, string extension, int maximumSizeInPixels)
    {
        using var skData = SKData.Create(File.OpenRead(imagePath));

        using var  skImage         = SKBitmap.Decode(skData);

        if (width == 0 || height == 0)
        {
            width  = skImage.Width;
            height = skImage.Height;
        }

        int        maxSizeInPixels = RestrictMaximumSizeInPixels(maximumSizeInPixels);
        Dimensions dimensions      = ImageDimensions(width, height, maxSizeInPixels);

        using SKBitmap? scaledBitmap = skImage.Resize(new SKImageInfo(dimensions.Width, dimensions.Height), SKSamplingOptions.Default);

        using var image = SKImage.FromBitmap(scaledBitmap);

        using SKData? encodedImage = image.Encode(SkiaSharpImageFormatMapping[extension], 50);

        var stream = new MemoryStream();
        encodedImage.SaveTo(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static FileDetail? ReadDb(FilesContext context, string directory, string filename)
    {
        try
        {
            return context.Files.Include(file => file.FileAccessDetail)
                          .FirstOrDefault(f => f.FileName == filename && f.DirectoryName == directory);
        }
        catch
        {
            _ = Task.Delay(TimeSpan.FromSeconds(2));

            return context.Files.Include(file => file.FileAccessDetail)
                          .FirstOrDefault(f => f.FileName == filename && f.DirectoryName == directory);
        }
    }

    [Refactor(5, 2, "There has to be a better solution")]
    private static Dimensions ImageDimensions(int width, int height, int maximumSizeInPixels)
    {
        int resizedImageWidth  = width;
        int resizedImageHeight = height;

        if (width < maximumSizeInPixels || height < maximumSizeInPixels)
        {
            resizedImageWidth  = width;
            resizedImageHeight = height;
        }
        else if (maximumSizeInPixels != 0)
        {
            resizedImageWidth  = maximumSizeInPixels;
            resizedImageHeight = maximumSizeInPixels;

            if (width > height)
            {
                resizedImageHeight = SetProportionalDimension(width, height, maximumSizeInPixels);
            }
            else
            {
                resizedImageWidth = SetProportionalDimension(height, width, maximumSizeInPixels);
            }
        }

        return new() { Height = resizedImageHeight, Width = resizedImageWidth, };
    }

    private static int SetProportionalDimension(int widthOrHeight, int heightOrWidth, int maximumSizeInPixels) =>
        Convert.ToInt32(heightOrWidth * maximumSizeInPixels / (double)widthOrHeight);

    private static int RestrictMaximumSizeInPixels(int maximumSizeInPixels) =>
        maximumSizeInPixels > MaximumHeightAndWidthForResizedImage
            ? MaximumHeightAndWidthForResizedImage
            : maximumSizeInPixels;
}
