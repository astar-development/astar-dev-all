// using AStar.Dev.Admin.Api.Client.Sdk.AdminApi;
// using AStar.Dev.Files.Api.Client.SDK.FilesApi;
// using AStar.Dev.Images.Api.Client.SDK.ImagesApi;

using AStar.Dev.Web.Components;
using AStar.Dev.Web.Fakes;
using Blazorise.LoadingIndicator;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Web;

namespace AStar.Dev.Web.Pages;

public partial class Dashboard
{
    private ApiStatusCheck?  adminApiStatusCheck;
    private ApiStatusCheck?  fileClassificationsApiStatusCheck;
    private ApiStatusCheck?  filesApiStatusCheck;
    private ApiStatusCheck?  imagesApiStatusCheck;
    private LoadingIndicator loadingIndicator = new();

    [Inject]
    public required AdminApiClient AdminApiClient { get; set; }

    [Inject]
    public required FilesApiClient FilesApiClient { get; set; }

    [Inject]
    public required ImagesApiClient ImagesApiClient { get; set; }

    [Inject]
    public required MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }

    [Inject]
    public required ILoggerAstar<Dashboard> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(Dashboard));

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CheckApiStatuses();
        }
    }

    private async Task CheckApiStatuses()
    {
        await loadingIndicator.Show();

        await adminApiStatusCheck!.UpdateStatusesAsync();
        await filesApiStatusCheck!.UpdateStatusesAsync();
        await fileClassificationsApiStatusCheck!.UpdateStatusesAsync();
        await imagesApiStatusCheck!.UpdateStatusesAsync();

        await loadingIndicator.Hide();
    }
}
