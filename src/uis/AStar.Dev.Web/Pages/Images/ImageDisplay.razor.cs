using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using static System.Int32;

namespace AStar.Dev.Web.Pages.Images;

public partial class ImageDisplay : ComponentBase
{
    private string ImageSource      { get; set;  } = string.Empty;

    private string FullNameWithPath { get; set; } = string.Empty;

    [Inject]
    public required ImagesApiClient ImagesApiClient { get; set; }

    [Inject]
    public required ILoggerAstar<ImageDisplay> Logger { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(ImageDisplay));

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return ;
        }

        await Task.Delay(1);
        var                              uri         = new Uri(Navigation.Uri);
        Logger.LogDebug("uri: {uri}", uri.ToString());

        Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("fullNameWithPath", out StringValues paramValue))
        {
            _ = queryParams.TryGetValue("width", out StringValues width);

            if (TryParse(width, out int widthInt))
            {
                if (widthInt <= 0)
                {
                    widthInt = 4000;
                }

                Logger.LogDebug("width: {width}",               widthInt);
                FullNameWithPath = paramValue.ToString();
                Logger.LogDebug("Query parameter: {ImageName}", FullNameWithPath);

                Stream result = await ImagesApiClient.GetImageAsync(FullNameWithPath, widthInt, false);

                if (result.Length == 0)
                {
                    Logger.LogError("Image {ImageName} result was null", FullNameWithPath);
                }
                else
                {
                    Logger.LogInformation("Image {ImageName} has been successfully retrieved", FullNameWithPath);
                }

                ImageSource = await PopulateImageFromStream(result);
                StateHasChanged();
            }
        }
    }

    private static async Task<string> PopulateImageFromStream(Stream stream)
    {
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        string b64String = Convert.ToBase64String(ms.ToArray());
        await stream.DisposeAsync();

        return $"data:image/png;base64,{b64String}";
    }
}
