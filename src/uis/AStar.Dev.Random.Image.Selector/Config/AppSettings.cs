namespace AStar.Dev.Random.Image.Selector.Config;

public sealed class AppSettings
{
    public Logging Logging { get; set; } = new();

    public string AllowedHosts { get; set; } = string.Empty;

    public ConnectionStrings ConnectionStrings { get; set; } = new();
}
