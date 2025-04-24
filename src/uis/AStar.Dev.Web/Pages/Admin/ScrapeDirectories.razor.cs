using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class ScrapeDirectories : ComponentBase
{
    [Inject]
    public required ILoggerAstar<ScrapeDirectories> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(ScrapeDirectories));

        await base.OnInitializedAsync();
    }
}
