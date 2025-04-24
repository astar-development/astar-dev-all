namespace AStar.Dev.Web.Fakes;

public class FilesApiClient
{
    public async Task<IReadOnlyCollection<DuplicateGroup>> GetDuplicateFilesAsync(SearchParameters searchParameters)
    {
        await Task.Delay(1);
        return null!;
    }

    public async Task GetDuplicateFilesCountAsync(SearchParameters searchParameters)
    {
        await Task.Delay(1);
    }
}
