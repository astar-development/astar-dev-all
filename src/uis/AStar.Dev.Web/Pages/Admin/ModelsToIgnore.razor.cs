
using AStar.Dev.Web.Fakes;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class ModelsToIgnore : ComponentBase
{
    [Inject]
    public required ILoggerAstar<ModelsToIgnore> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(ModelsToIgnore));

        await base.OnInitializedAsync();
    }
}
