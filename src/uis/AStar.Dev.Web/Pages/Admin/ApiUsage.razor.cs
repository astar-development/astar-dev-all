using AStar.Dev.Web.Fakes;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Admin;

public partial class ApiUsage : ComponentBase
{
    private readonly List<string> backgroundColors =
    [
        ChartColor.FromRgba( 255, 99,  132, 0.2f ), ChartColor.FromRgba( 54,  162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ),
        ChartColor.FromRgba( 75,  192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ),
    ];

    private readonly List<string> borderColors =
    [
        ChartColor.FromRgba( 255, 99,  132, 1f ), ChartColor.FromRgba( 54,  162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ),
        ChartColor.FromRgba( 75,  192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ),
    ];

    private readonly string[] Labels = [ "Red", "Blue", "Yellow", "Green", "Purple", "Orange", ];

    private LineChart<double>                       lineChart = new();
    private bool                                    loading;
    private Dictionary<string, List<ApiUsageEvent>> usageEvents = [];

    [Inject]
    private UsageApiClient UsageApiClient { get; set; } = null!;

    [Inject]
    public required ILoggerAstar<ApiUsage> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(ApiUsage));

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await HandleRedraw();

            loading = true;
            StateHasChanged();
            usageEvents = await UsageApiClient.GetApiUsageEventsAsync();
            loading     = false;
            StateHasChanged();
        }
    }

    private async Task HandleRedraw()
    {
        await lineChart.Clear();

        await lineChart.AddLabelsDatasetsAndUpdate( Labels, GetLineChartDataset() );
    }

    private LineChartDataset<double> GetLineChartDataset()
        => new()
        {
            Label                  = "# of randoms",
            Data                   = RandomizeData(),
            BackgroundColor        = backgroundColors,
            BorderColor            = borderColors,
            Fill                   = true,
            PointRadius            = 3,
            CubicInterpolationMode = "monotone",
        };

    private  List<double> RandomizeData()
    {
        var r = new Random( DateTime.Now.Millisecond );

        return
        [
            r.Next( 3, 50 ) * r.NextDouble(),
            r.Next( 3, 50 ) * r.NextDouble(),
            r.Next( 3, 50 ) * r.NextDouble(),
            r.Next( 3, 50 ) * r.NextDouble(),
            r.Next( 3, 50 ) * r.NextDouble(),
            r.Next( 3, 50 ) * r.NextDouble(),
        ];
    }
}
