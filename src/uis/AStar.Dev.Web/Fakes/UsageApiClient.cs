namespace AStar.Dev.Web.Fakes;

internal class UsageApiClient
{
    public async Task<Dictionary<string, List<ApiUsageEvent>>> GetApiUsageEventsAsync()
    {
        return await Task.FromResult(new Dictionary<string, List<ApiUsageEvent>>());
    }
}
