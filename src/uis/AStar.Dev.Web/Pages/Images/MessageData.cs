namespace AStar.Dev.Web.Pages.Images;

internal sealed record MessageData(string Message)
{
    private readonly DateTime messageDateTime = DateTime.UtcNow;

    public           string   MessageForDisplay => $"{messageDateTime:R} - {Message}";
}
