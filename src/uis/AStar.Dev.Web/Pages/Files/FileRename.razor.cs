using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Files;

public partial class FileRename : ComponentBase
{
    [Inject]
    public required ILoggerAstar<FileRename> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(FileRename));

        await base.OnInitializedAsync();
    }
}
