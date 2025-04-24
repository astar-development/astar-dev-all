using Blazorise;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Shared;

public partial class MainLayout
{
    protected string LayoutType = "fixed-header";

    [Inject]
    protected ITextLocalizerService? LocalizationService { get; set; }

    [CascadingParameter]
    protected Theme? Theme { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await SelectCulture("en-US");

        await base.OnInitializedAsync();

        if (Theme is not null)
        {
            Theme.BackgroundOptions.Primary   = SupportedColours.White;
            Theme.ColorOptions.Primary        = SupportedColours.Black;
            Theme.BodyOptions.BackgroundColor = SupportedColours.White;
            Theme.BodyOptions.TextColor       = SupportedColours.Black;
            Theme.IsRounded                   = true;

            await InvokeAsync(Theme.ThemeHasChanged);
        }
    }

    private Task SelectCulture(string name)
    {
        LocalizationService!.ChangeLanguage(name);

        return Task.CompletedTask;
    }

    private Task OnThemeEnabledChanged(bool value)
    {
        if (Theme is null)
        {
            return Task.CompletedTask;
        }

        Theme.Enabled = value;

        return InvokeAsync(Theme.ThemeHasChanged);
    }

    private Task OnThemeGradientChanged(bool value)
    {
        if (Theme is null)
        {
            return Task.CompletedTask;
        }

        Theme.IsGradient = value;

        return InvokeAsync(Theme.ThemeHasChanged);
    }

    private Task OnThemeRoundedChanged(bool value)
    {
        if (Theme is null)
        {
            return Task.CompletedTask;
        }

        Theme.IsRounded = value;

        return InvokeAsync(Theme.ThemeHasChanged);
    }

    private Task OnThemeColorChanged(string value)
    {
        if (Theme is null)
        {
            return Task.CompletedTask;
        }

        Theme.ColorOptions ??= new();

        Theme.BackgroundOptions ??= new();

        Theme.TextColorOptions ??= new();

        Theme.ColorOptions.Primary      = value;
        Theme.BackgroundOptions.Primary = value;
        Theme.TextColorOptions.Primary  = value;

        Theme.InputOptions ??= new();

        Theme.InputOptions.CheckColor  = value;
        Theme.InputOptions.SliderColor = value;

        Theme.BodyOptions                 ??= new();
        Theme.BodyOptions.BackgroundColor =   "#FFFFFF";
        Theme.BodyOptions.TextColor       =   "#000000";

        Theme.SpinKitOptions ??= new();

        Theme.SpinKitOptions.Color = value;

        return InvokeAsync(Theme.ThemeHasChanged);
    }
}
