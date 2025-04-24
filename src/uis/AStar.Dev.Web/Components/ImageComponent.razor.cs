using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components;

public partial class ImageComponent : ComponentBase
{
    private string imageSource = string.Empty;

    [Parameter]
    public string ImageName { get; set; } = string.Empty;

    [Parameter]
    public string ImageFullName { get; set; } = string.Empty;

    [Parameter]
    public int ImageSize { get; set; } = 150;

    [Inject]
    public required ImagesApiClient ImagesApiClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Stream image = await ImagesApiClient.GetImageAsync(ImageFullName, ImageSize, true);

        imageSource = await PopulateImageFromStream(image);
    }

    private async Task<string> PopulateImageFromStream(Stream stream)
    {
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        string b64String = Convert.ToBase64String(ms.ToArray());
        await stream.DisposeAsync();

        return $"data:image/png;base64,{b64String}";
    }
}
