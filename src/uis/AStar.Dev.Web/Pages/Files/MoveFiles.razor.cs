using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Files;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class MoveFiles : ComponentBase
{
    [Inject]
    public required ILoggerAstar<MoveFiles> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(MoveFiles));

        await base.OnInitializedAsync();
    }
}
