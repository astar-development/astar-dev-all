using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

public partial class AddFilesToDatabase : ComponentBase
{
    // [Inject]
    // public required ILoggerAstar<SiteConfiguration> Logger { get; set; }
    //
    // [Inject]
    // private FilesApiClient FilesApiClient { get; set; } = null!;

    private string?   Directory   { get; set; } = "sdsd";
    private string[]? Directories { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
       // Logger.LogPageView(nameof(AddFilesToDatabase));

        await base.OnInitializedAsync();
    }

    private async Task GetDirectories()
    {
        await Task.CompletedTask;
        //IEnumerable<string> x = await FilesApiClient.GetDirectoriesAsync(Directory!, CancellationToken.None);

        //Directories = x.ToArray();

        StateHasChanged();
    }
}
