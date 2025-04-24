using AStar.Dev.Logging.Extensions;
using AStar.Dev.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AStar.Dev.Web.Pages.Admin;

public partial class SiteConfigurationUpdate : ComponentBase
{
    private string repeatPassword = string.Empty;

    [SupplyParameterFromQuery]
    public required string SiteConfigurationSlug { get; set; }

    [Inject]
    public required ILoggerAstar<SiteConfiguration> Logger { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(SiteConfigurationUpdate));

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        Logger.LogInformation("After render-SiteConfigurationSlug: {SiteConfigurationSlug}", SiteConfigurationSlug);

        return base.OnAfterRenderAsync(firstRender);
    }

    private IEnumerable<string> PasswordStrength(string passwordToCheck)
    {
        if (string.IsNullOrWhiteSpace(passwordToCheck))
        {
            yield return
                "Please specify a password with at least 8 characters, at least one capital letter, at least one lowercase letter, at least one digit, and at least one special character.";

            yield break;
        }

        if (passwordToCheck.Length < 8)
        {
            yield return "Please specify a password with at least 8 characters.";
        }

        if (!passwordToCheck.ContainsAtLeastOneUppercaseLetter())
        {
            yield return "Please specify a password that contains at least one capital letter.";
        }

        if (!passwordToCheck.ContainsAtLeastOneLowercaseLetter())
        {
            yield return "Please specify a password that contains at least one lowercase letter.";
        }

        if (!passwordToCheck.ContainsAtLeastOneDigit())
        {
            yield return "Please specify a password that contains at least one digit.";
        }

        if (!passwordToCheck.ContainsAtLeastOneSpecialCharacter())
        {
            yield return "Please specify a password that contains at least one special character.";
        }
    }

    private Task HandleValidSubmit(EditContext arg) =>

        //siteConfigurations.Username = repeatPassword;
        Task.CompletedTask;
}
