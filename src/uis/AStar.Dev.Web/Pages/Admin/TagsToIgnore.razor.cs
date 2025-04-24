
using AStar.Dev.Web.Fakes;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class TagsToIgnore : ComponentBase
{
    [Inject]
    public required ILoggerAstar<TagsToIgnore> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(TagsToIgnore));

        await base.OnInitializedAsync();
    }
}
