namespace AStar.Dev.Web.Fakes;

public class ImagesApiClient
{
    public async Task<Stream> GetImageAsync(string fullNameWithPath, int widthInt, bool b)
    {
        await Task.Delay(0);
        return null!;
    }
}
