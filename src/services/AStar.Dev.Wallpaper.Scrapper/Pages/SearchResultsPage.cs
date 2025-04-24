﻿using Microsoft.Playwright;
using Serilog.Core;

namespace AStar.Dev.Wallpaper.Scrapper.Pages;

public class SearchResultsPage(IPage page, Logger logger)
{
    private ILocator NewSubscriptionWallpapersHeader => page.GetByText("New Subscription Wallpapers", new() { Exact = false, });

    private ILocator WallpaperSearchHeader => page.GetByText("Wallpapers found for", new() { Exact = false, });

    private ILocator WallpaperSearchHeaderGeneral => page.GetByText("Wallpapers found", new() { Exact = false, });

    private ILocator ImagePreviews => page.GetByRole(AriaRole.Link);

    public async Task<IResponse?> LoadSearchPageAsync(string searchString, int pageNumber)
    {
        try
        {
            return await GotoSearchPageAsync(searchString, pageNumber);
        }
        catch (Exception exception)
        {
            logger.Error(exception.GetBaseException().Message);

            throw;
        }
    }

    public async Task<(int pageCount, int imageCount, string subDirectoryName)> PageInfoAsync()
    {
        try
        {
            return await GetPageInfoAsync();
        }
        catch (Exception exception)
        {
            logger.Error(exception.GetBaseException().Message);

            throw;
        }
    }

    public async Task<IReadOnlyCollection<string>> ImagePageLinksAsync()
    {
        try
        {
            return await GetTheImagePageLinksAsync();
        }
        catch (Exception exception)
        {
            logger.Error(exception.GetBaseException().Message);

            throw;
        }
    }

    private async Task<IResponse?> GotoSearchPageAsync(string searchString, int pageNumber)
    {
        IResponse? searchPage = await GotoPageAsync(searchString, pageNumber);

        return searchPage is { Ok: true, } ? searchPage : await GotoPageAsync(searchString, pageNumber);
    }

    private async Task<IReadOnlyCollection<string>> GetTheImagePageLinksAsync()
    {
        List<string>            wantedLinks   = new();
        IReadOnlyList<ILocator> imagePreviews = await ImagePreviews.AllAsync();

        foreach (ILocator imagePreview in imagePreviews)
        {
            string? hrefString = await imagePreview.GetAttributeAsync("href");

            if (hrefString != null && hrefString.Contains("/w/"))
            {
                wantedLinks.Add(hrefString);
            }
        }

        return wantedLinks.Take(24).ToList();
    }

    private async Task<(int pageCount, int imageCount, string subDirectoryName)> GetPageInfoAsync()
    {
        string? text = await GetPageHeader();

        if (text is null)
        {
            return (0, 0, string.Empty);
        }

        int    firstSpaceIndex  = text.IndexOf(" ", StringComparison.Ordinal);
        int    hashIndex        = text.IndexOf("for ", StringComparison.Ordinal) + 3;
        string subDirectoryName = string.Empty;

        if (hashIndex > 0)
        {
            subDirectoryName = text[(hashIndex + 1)..].Replace(" ", "-").Replace("#", string.Empty);
        }

        string searchResults = text.Replace(",", string.Empty)[..firstSpaceIndex];
        var    imageCount    = decimal.Parse(searchResults);

        return (Convert.ToInt32(Math.Ceiling(imageCount / 24)), (int)imageCount, subDirectoryName);
    }

    private async Task<IResponse?> GotoPageAsync(string searchString, int pageNumber) =>
        await page.GotoAsync($"{searchString}{pageNumber}", new() { Timeout = 60000, });

    private async Task<string?> GetPageHeader()
    {
        string? text;

        try
        {
            text = await WallpaperSearchHeader.TextContentAsync();
        }
        catch
        {
            text = await WallpaperSearchHeaderGeneral.TextContentAsync();
        }

        if (text?.Length == 0)
        {
            text = await NewSubscriptionWallpapersHeader.TextContentAsync();
        }

        return text;
    }
}
