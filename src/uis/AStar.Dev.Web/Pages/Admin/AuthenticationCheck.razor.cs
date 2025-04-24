using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

public partial class AuthenticationCheck : ComponentBase
{
    [Inject]
    public required ILoggerAstar<ApiUsage> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(AuthenticationCheck));

        await base.OnInitializedAsync();
    }
}
