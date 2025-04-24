using AStar.Dev.Web.Fakes;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class SiteConfiguration
{
    private bool                            loading;
    private IEnumerable<Fakes.SiteConfiguration> siteConfigurations = new List<Fakes.SiteConfiguration>();

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
        siteConfigurations = (IEnumerable<Fakes.SiteConfiguration>)await AdminApiClient.GetSiteConfigurationAsync();
        loading            = false;
        StateHasChanged();
    }
}
