using System.Security.Claims;
using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace AStar.Dev.Web.Pages.ToDoPages;

public class ToDoListBase : ComponentBase
{
    private readonly ToDo toDo = new() { Title = "", };

    protected IEnumerable<ToDo> ToDoList = new List<ToDo>();

    [Inject]
    public required MicrosoftIdentityConsentAndConditionalAccessHandler ConsentHandler { get; set; }

    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public required IDownstreamApi DownstreamApi { get; set; }

    [Inject]
    public required ILoggerAstar<ToDoList> Logger { get; set; }

    protected override async Task OnInitializedAsync() =>
        await GetToDoListService();

    [AuthorizeForScopes(ScopeKeySection = "TodoList:TodoListScope")]
    private async Task GetToDoListService()
    {
        try
        {
            AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            ClaimsPrincipal user = authState.User;
            Logger.LogInformation("Getting to-do list for: {User}", user.Identity?.Name);

            ToDoList = (await DownstreamApi
                           .GetForUserAsync<IEnumerable<ToDo>>("TodoList", options =>
                                                                           {
                                                                               options.Scopes =
                                                                               [
                                                                                   "api://2ca26585-5929-4aae-86a7-a00c3fc2d061/ToDoList.Read",
                                                                                   "api://2ca26585-5929-4aae-86a7-a00c3fc2d061/ToDoList.Write",
                                                                               ];
                                                                           },
                                                               user))!;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Seems like something went wrong...darned cert error...");

            ConsentHandler.HandleException(ex);
        }
    }

    /// <summary>
    ///     Deletes the selected item then retrieves the todo list.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected async Task DeleteItem(int id)
    {
        await DownstreamApi.DeleteForUserAsync(
                                               "TodoList", toDo,
                                               options => options.RelativePath = $"api/todolist/{id}");

        await GetToDoListService();
    }
}
