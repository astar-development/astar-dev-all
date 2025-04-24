namespace AStar.Dev.Web.Pages.ToDoPages;

public sealed class ToDo
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Owner { get; set; }
}
