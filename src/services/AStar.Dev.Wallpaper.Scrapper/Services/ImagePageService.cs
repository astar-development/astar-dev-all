using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Wallpaper.Scrapper.Models;
using AStar.Dev.Wallpaper.Scrapper.Pages;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;

namespace AStar.Dev.Wallpaper.Scrapper.Services;

public class ImagePageService(ImagePage imagePage, ConnectionStrings connectionStrings, Logger logger)
{
    public async Task GetTheImagePagesAsync(IReadOnlyCollection<string> imagePageLinks)
    {
        foreach (string pageLink in imagePageLinks)
        {
            try
            {
                int    indexOfFinalSlash = pageLink.LastIndexOf("/", StringComparison.Ordinal) + 1;
                string fileName          = pageLink[indexOfFinalSlash..];

                using var context = new FilesContext(new() { Value = connectionStrings.SqlServer, }, new());

                if (await context.Files.FirstOrDefaultAsync(fileInfoJb => fileInfoJb.FileName.Contains(fileName)) != null)
                {
                    logger.Information("Not downloading {fileName} as we already have it...", fileName);

                    continue;
                }

                await imagePage.GetImageFromPage(pageLink);
            }
            catch
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                await imagePage.GetImageFromPage(pageLink);
            }
        }
    }
}
