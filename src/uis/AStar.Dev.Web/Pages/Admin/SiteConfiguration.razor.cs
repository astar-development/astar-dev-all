using AStar.Dev.Admin.Api.Client.Sdk.AdminApi;
using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;
using SiteConfigurations = AStar.Dev.Admin.Api.Client.Sdk.Models.SiteConfiguration;

namespace AStar.Dev.Web.Pages.Admin;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class SiteConfiguration
{
    private bool                            loading;
    private IEnumerable<SiteConfigurations> siteConfigurations = new List<SiteConfigurations>();

    [Inject]
    private AdminApiClient AdminApiClient { get; set; } = null!;

    [Inject]
    public required ILoggerAstar<SiteConfiguration> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(SiteConfiguration));

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        loading = true;
        StateHasChanged();
        siteConfigurations = await AdminApiClient.GetSiteConfigurationAsync();
        loading            = false;
        StateHasChanged();
    }
}
