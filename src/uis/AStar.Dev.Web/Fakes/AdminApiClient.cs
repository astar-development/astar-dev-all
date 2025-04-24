using AStar.Dev.Web.Pages.Admin;

namespace AStar.Dev.Web.Fakes;

public class AdminApiClient
{
    public async Task<IEnumerable<Fakes.SiteConfiguration>> GetSiteConfigurationAsync()
    {
        await Task.Delay(0);
        return null!;
    }
}
