using AStar.Dev.Wallpaper.Scrapper.Models;
using Microsoft.Playwright;

namespace AStar.Dev.Wallpaper.Scrapper.Pages;

public class TopWallpapersPage(IPage page, SearchConfiguration searchConfiguration)
{
    private ILocator PageCount => page.GetByText("Page ", new () { Exact = false, });

    private ILocator ImagePreviews => page.GetByRole(AriaRole.Link);

    public async Task<IResponse?> LoadTopWallpapersPageAsync(int pageNumber) =>
        _ = await page.GotoAsync($"{searchConfiguration.TopWallpapers}{pageNumber}");

    public async Task<int> PageInfoAsync()
    {
        string? text = await PageCount.First.TextContentAsync();

        if (text is null)
        {
            return 0;
        }

        int    firstSlashIndex = text.IndexOf("/", StringComparison.Ordinal) + 1;
        string pages           = text[firstSlashIndex..].Trim();

        return Convert.ToInt32(pages);
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
}
