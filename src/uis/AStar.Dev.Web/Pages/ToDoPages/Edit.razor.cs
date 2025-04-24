using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Abstractions;

namespace AStar.Dev.Web.Pages.ToDoPages;

public partial class Edit : ComponentBase
{
    private ToDo toDo = new()
                        {
                            Title = null!,
                        };

    [Parameter]
    public int Id { get; set; }

    [Inject]
    public required ILoggerAstar<Dashboard> Logger { get; set; }

    [Inject]
    public required NavigationManager Navigation { get; set; }

    [Inject]
    public required IDownstreamApi DownstreamApi { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(Dashboard));

        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        toDo = (await DownstreamApi.GetForUserAsync<ToDo>(
                                                          "TodoList",
                                                          options => options.RelativePath = $"api/todolist/{Id}"))!;
    }

    private async Task EditTask()
    {
        await DownstreamApi.PatchForUserAsync<ToDo, ToDo>(
                                                          "TodoList", toDo,
                                                          options => options.RelativePath = $"api/todolist/{Id}");

        Navigation.NavigateTo("todo-list");
    }
}
