using AStar.Dev.Images.Api.Config;

namespace AStar.Dev.Images.Api.ApiClients;

/// <summary>
/// </summary>
public sealed class FilesApiClient
{
    private readonly HttpClient httpClient;

    /// <summary>
    /// </summary>
    public FilesApiClient(HttpClient httpClient) =>
        this.httpClient = httpClient;

    /// <summary>
    /// </summary>
    public async Task<HttpResponseMessage> GetFileListAsync(SearchParameters searchParameters) =>
        await httpClient.GetAsync($"api/files?{searchParameters}");

    /// <summary>
    /// </summary>
    public async Task<HttpResponseMessage> GetFileListCountAsync(SearchParameters searchParameters) =>
        await httpClient.GetAsync($"api/filesCount?{searchParameters}");
}
