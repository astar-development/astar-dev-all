
using AStar.Dev.Web.Fakes;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Images;

public partial class ScrapeImages : ComponentBase
{
    [Inject]
    public required ILoggerAstar<ScrapeImages> Logger { get; set; }

    private IList<MessageData> Messages { get;  } = [];

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(ScrapeImages));

        Messages.Insert(0, new("Awaiting scrape updates..."));
        await base.OnInitializedAsync();
    }
}
