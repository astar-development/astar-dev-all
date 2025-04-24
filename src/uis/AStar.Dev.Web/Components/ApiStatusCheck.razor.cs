using AStar.Dev.Api.HealthChecks;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Web;
using Constants = AStar.Dev.Web.Infrastructure.Constants;

namespace AStar.Dev.Web.Components;

public partial class ApiStatusCheck(MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler)
{
    private const string NotChecked = "Not checked yet...";

    private const string CheckingText = "Checking...please wait...";

    private const string WarningClass = "alert alert-warning status";

    private const string HealthCheckFailure = "alert alert-danger status";

    private const string HealthCheckSuccess = "alert alert-success status";

    private const string HealthCheckWarning = WarningClass;

    private readonly Dictionary<string, string> images = new()
                                                         {
                                                             { Constants.ApiNames.AdminApi, "assets/ai-generated-8001027_640.jpg" },
                                                             { Constants.ApiNames.FilesApi, "assets/ai-generated-8165290_640.jpg" },
                                                             { Constants.ApiNames.FilesClassificationsApi, "assets/ai-generated-8460800_640.jpg" },
                                                             { Constants.ApiNames.ImagesApi, "assets/cyborg-8829739_640.png" },
                                                         };

    public string ApiHealthCheckClass { get; set; } = WarningClass;

    public string ApiHealthStatus { get; set; } = NotChecked;

    public string ImageSource { get; set; } = string.Empty;

    [Parameter]
    public string ApiName { get; set; } = string.Empty;

    [Parameter]
    public required IApiClient ApiClient { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
        => await UpdateStatusesAsync();

    internal async Task UpdateStatusesAsync()
    {
        try
        {
            ApiHealthStatus = CheckingText;
            ImageSource     = images[ApiName];

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            HealthStatusResponse apiStatus = await ApiClient.GetHealthAsync(CancellationToken.None);
            ApiHealthCheckClass = SetHealthCheckClass(apiStatus);
            ApiHealthStatus     = apiStatus.Status;
            await base.OnInitializedAsync();
        }
        catch (Exception ex)
        {
            consentHandler.HandleException(ex);
        }
    }

    private static string SetHealthCheckClass(HealthStatusResponse? healthStatusResponse)
        => healthStatusResponse?.Status == "Healthy"
            ? HealthCheckSuccess
            : HealthCheckFailure;
}
