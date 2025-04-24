using AStar.Dev.Wallpaper.Scrapper.Models;
using Microsoft.Playwright;

namespace AStar.Dev.Wallpaper.Scrapper.Pages;

public class SubscriptionsImagesListPage(IPage page, SearchConfiguration searchConfiguration)
{
    private ILocator ImagePreviews => page.GetByRole(AriaRole.Link);

    private ILocator NewSubscriptionWallpapersHeader => page.GetByText("New Subscription Wallpapers", new() { Exact = false, });

    public async Task<IResponse?> LoadSubscriptionResultsPageAsync(int pageNumber) =>
        _ = await page.GotoAsync($"{searchConfiguration.Subscriptions}{pageNumber}");

    public async Task<(int pageCount, string subDirectoryName)> PageInfoAsync()
    {
        string? text = await NewSubscriptionWallpapersHeader.TextContentAsync();

        if (text is null)
        {
            return (0, string.Empty);
        }

        int    firstSpaceIndex  = text.IndexOf(" ",   StringComparison.Ordinal);
        int    hashIndex        = text.IndexOf("New", StringComparison.Ordinal);
        string subDirectoryName = string.Empty;

        if (hashIndex > 0)
        {
            subDirectoryName = text[hashIndex..].Replace(" ", "-").Replace("#", string.Empty);
        }

        string  searchResults = text.Replace(",", string.Empty)[..firstSpaceIndex];
        decimal imageCount    = decimal.Parse(searchResults) / 24;

        return (Convert.ToInt32(Math.Ceiling(imageCount)), subDirectoryName);
    }

    public async Task<IReadOnlyCollection<string>> GetImagePageLinks()
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

    public async Task Clear() =>
        await page.Locator("div")
                  .Filter(new() { HasText                = " Clear All Subscriptions", })
                  .GetByRole(AriaRole.Link, new() { Name = " Clear All Subscriptions", })
                  .ClickAsync();
}
