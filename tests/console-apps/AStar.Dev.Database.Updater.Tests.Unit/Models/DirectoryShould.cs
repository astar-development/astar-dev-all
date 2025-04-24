namespace AStar.Dev.Database.Updater.Models;

public sealed class DirectoryShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new Directory("", "")
           .ToString()
           .ShouldMatchApproved();
}
