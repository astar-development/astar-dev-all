using AStar.Dev.Web.Shared;
using Blazorise;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Layout;

public partial class TopMenu
{
    [Parameter]
    public EventCallback<bool> ThemeEnabledChanged { get; set; }

    [Parameter]
    public EventCallback<bool> ThemeGradientChanged { get; set; }

    [Parameter]
    public EventCallback<bool> ThemeRoundedChanged { get; set; }

    [Parameter]
    public EventCallback<string> ThemeColorChanged { get; set; }

    [Parameter]
    public string? LayoutType { get; set; }

    [Parameter]
    public EventCallback<string> LayoutTypeChanged { get; set; }

    [Inject]
    protected ITextLocalizerService? LocalizationService { get; set; }

    [CascadingParameter]
    protected Theme? Theme { get; set; }

    private async Task SwapLightAndDarkThemes()
    {
        if (Theme is not null)
        {
            if (Theme.BackgroundOptions.Primary == SupportedColours.Black)
            {
                Theme.BackgroundOptions.Primary   = SupportedColours.White;
                Theme.ColorOptions.Primary        = SupportedColours.Black;
                Theme.BodyOptions.BackgroundColor = SupportedColours.White;
                Theme.BodyOptions.TextColor       = SupportedColours.Black;
            }
            else
            {
                Theme.BackgroundOptions.Primary   = SupportedColours.Black;
                Theme.ColorOptions.Primary        = SupportedColours.White;
                Theme.BodyOptions.BackgroundColor = SupportedColours.Black;
                Theme.BodyOptions.TextColor       = SupportedColours.White;
            }

            await InvokeAsync(Theme.ThemeHasChanged);
        }
    }
}
