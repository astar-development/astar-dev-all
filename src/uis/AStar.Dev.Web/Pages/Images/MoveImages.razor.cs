using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Images;

public partial class MoveImages : ComponentBase
{
    [Inject]
    public required ILoggerAstar<MoveImages> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(MoveImages));

        await base.OnInitializedAsync();
    }
}
