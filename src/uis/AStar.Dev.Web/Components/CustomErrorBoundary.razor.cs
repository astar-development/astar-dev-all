using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AStar.Dev.Web.Components;

public partial class CustomErrorBoundary : ErrorBoundary
{
    [Inject]
    public required ILogger<CustomErrorBoundary> Logger { get; set; }

    [Inject]
    public required IHttpContextAccessor HttpContextAccessor { get; set; }

    private string? RequestId     { get; set; }
    private bool    ShowRequestId => !string.IsNullOrEmpty(RequestId);

    protected override Task OnErrorAsync(Exception ex)
    {
        HttpContext? context = HttpContextAccessor.HttpContext;
        RequestId = Activity.Current?.Id ?? context?.TraceIdentifier;
        Logger.LogError(ex, "😈 An error occurred in the UI: {ErrorMessage}", ex.GetBaseException().Message);

        return Task.CompletedTask;
    }
}
