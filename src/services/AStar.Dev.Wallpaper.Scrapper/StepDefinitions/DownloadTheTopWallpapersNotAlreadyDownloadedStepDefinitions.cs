using AStar.Dev.Wallpaper.Scrapper.Models;
using AStar.Dev.Wallpaper.Scrapper.Pages;
using AStar.Dev.Wallpaper.Scrapper.Services;
using AStar.Dev.Wallpaper.Scrapper.Support;
using Microsoft.Playwright;
using Serilog.Core;
using TechTalk.SpecFlow;

namespace AStar.Dev.Wallpaper.Scrapper.StepDefinitions;

[Binding]
public class DownloadTheTopWallpapersNotAlreadyDownloadedStepDefinitions(
    TopWallpapersPage   topWallpapersPage,
    ImagePageService    imagePageService,
    SearchConfiguration searchConfiguration,
    ConfigurationSaver  configurationSaver,
    Logger              logger)
{
    [Then("I can download the top wallpapers I do not already have")]
    public async Task ThenICanDownloadTheTopWallpapersIDoNotAlreadyHave()
    {
        try
        {
            await GetTheNewTopWallpapersAsync();
        }
        catch (Exception exception)
        {
            logger.Error(exception.GetBaseException().Message);

            throw;
        }
    }

    private async Task GetTheNewTopWallpapersAsync()
    {
        IResponse? pageDetails = await topWallpapersPage.LoadTopWallpapersPageAsync(searchConfiguration.TopWallpapersStartingPageNumber);

        if (pageDetails is { Ok: false, })
        {
            _ = await topWallpapersPage.LoadTopWallpapersPageAsync(1);
        }

        string body      = await pageDetails.TextAsync();
        int    pageCount = await topWallpapersPage.PageInfoAsync();
        logger.Information("There are a total of {TopWallpapersPageCount} pages for the Top Wallpapers.", pageCount);
        searchConfiguration.TopWallpapersTotalPages = pageCount;

        configurationSaver.SaveUpdatedConfiguration();

        for (int currentPageNumber = searchConfiguration.TopWallpapersStartingPageNumber;
             currentPageNumber <= searchConfiguration.TopWallpapersTotalPages;
             currentPageNumber++)
        {
            int delay = Random.Shared.Next(searchConfiguration.ImagePauseInSeconds, searchConfiguration.ImagePauseInSeconds + 4);
            Thread.Sleep(TimeSpan.FromSeconds(delay));
            searchConfiguration.TopWallpapersStartingPageNumber = currentPageNumber;
            configurationSaver.SaveUpdatedConfiguration();
            _ = await topWallpapersPage.LoadTopWallpapersPageAsync(searchConfiguration.TopWallpapersStartingPageNumber);
            IReadOnlyCollection<string> imagePageLinks = await topWallpapersPage.GetImagePageLinks();

            await imagePageService.GetTheImagePagesAsync(imagePageLinks);
        }
    }
}
