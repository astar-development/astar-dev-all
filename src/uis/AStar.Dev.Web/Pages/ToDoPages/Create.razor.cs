using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace AStar.Dev.Web.Pages.ToDoPages;

public partial class Create : ComponentBase
{
    private readonly ToDo toDo = new()
                                 {
                                     Title = null!,
                                 };

    [Inject]
    public required ILoggerAstar<Dashboard> Logger { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    [Inject]
    public required IDownstreamApi DownstreamApi { get; set; }

    [Inject]
    public required MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }

    [Inject]
    public required IConfiguration Configuration { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(Dashboard));

        await base.OnInitializedAsync();
    }

    private async Task AddTask()
    {
        try
        {
            await DownstreamApi.PostForUserAsync("TodoList", toDo);
            Navigation.NavigateTo("todo-list");
        }
        catch (MsalUiRequiredException)
        {
            string[] scopes = [Configuration.GetSection("AzureAd")["Scopes"]!,];
            ConsentHandler.ChallengeUser(scopes);
        }
    }
}
