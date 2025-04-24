using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class SearchConfiguration : ComponentBase
{
    [Inject]
    public required ILoggerAstar<SearchConfiguration> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(SearchConfiguration));

        await base.OnInitializedAsync();
    }
}
