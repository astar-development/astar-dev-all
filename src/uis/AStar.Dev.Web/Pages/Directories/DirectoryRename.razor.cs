using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Directories;

public partial class DirectoryRename : ComponentBase
{
    [Inject]
    public required ILoggerAstar<DirectoryRename> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(DirectoryRename));

        await base.OnInitializedAsync();
    }
}
