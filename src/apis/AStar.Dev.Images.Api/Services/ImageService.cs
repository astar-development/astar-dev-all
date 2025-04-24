using System.Drawing;
using AStar.Dev.Images.Api.Models;
using SkiaSharp;

#pragma warning disable CA1416
#pragma warning disable CS0618

namespace AStar.Dev.Images.Api.Services;

/// <summary>
/// </summary>
/// <param name="logger"></param>
public sealed class ImageService(ILogger<ImageService> logger) : IImageService
{
    private const int MaximumHeightAndWidthForResizedImage = 850;

    /// <inheritdoc />
    public Stream GetImage(string imagePath, int maxDimensions)
    {
        logger.LogInformation("Getting resized {ImagePath}", imagePath);
        using var  imageFromFile = SKImage.FromEncodedData(imagePath);
        Dimensions dimensions    = ImageDimensions(imageFromFile.Width, imageFromFile.Height, maxDimensions);
        var        info          = new SKImageInfo(dimensions.Width, dimensions.Height, SKColorType.Bgra8888);
        var        output        = SKImage.Create(info);
        _ = imageFromFile.ScalePixels(output.PeekPixels(), SKFilterQuality.None);
        using SKData? data = output.Encode(SKEncodedImageFormat.Jpeg, 50);

        return data.AsStream();
    }

    /// <summary>
    /// </summary>
    public Image GetImage2(string imagePath, int maxDimensions)
    {
        try
        {
            return Image.FromFile(imagePath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred ({Error}) whilst retrieving {FileName} - full stack: {Stack}",
                            e.Message, imagePath, e.StackTrace);

            var bmp = new Bitmap(100, 50);
            var g   = Graphics.FromImage(bmp);
            g.Clear(Color.DarkRed);

            return bmp;
        }
    }

    private static Dimensions ImageDimensions(int width, int height, int maximumSizeInPixels)
    {
        int restrictedSizeInPixels = maximumSizeInPixels > MaximumHeightAndWidthForResizedImage
                                         ? MaximumHeightAndWidthForResizedImage
                                         : maximumSizeInPixels;

        double ratio     = (double)restrictedSizeInPixels / width;
        var    newWidth  = (int)(width  * ratio);
        var    newHeight = (int)(height * ratio);

        return new() { Height = newHeight, Width = newWidth, };
    }
}
