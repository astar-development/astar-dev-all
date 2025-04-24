using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Utilities;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components;

public partial class ImageDetailDisplay : ComponentBase
{
    [Parameter]
    public int ImageSize { get; set; }

    [Parameter]
    public required FileDetail FileDetail { get; set; }

    [Parameter]
    public required string RootDirectory { get; set; }

    [Inject]
    public required ILogger<ImageDetailDisplay> Logger { get; set; }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        Logger.LogDebug("FileDetail {FileDetail}", FileDetail.ToJson());
    }
}
