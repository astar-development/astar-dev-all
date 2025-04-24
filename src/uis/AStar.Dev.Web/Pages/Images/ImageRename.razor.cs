using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Images;

public partial class ImageRename : ComponentBase
{
    [Inject]
    public required ILoggerAstar<ImageRename> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(ImageRename));

        await base.OnInitializedAsync();
    }
}
