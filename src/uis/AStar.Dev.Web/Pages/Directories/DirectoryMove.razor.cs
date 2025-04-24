using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Directories;

public partial class DirectoryMove : ComponentBase
{
    [Inject]
    public required ILoggerAstar<DirectoryMove> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(DirectoryMove));

        await base.OnInitializedAsync();
    }
}
