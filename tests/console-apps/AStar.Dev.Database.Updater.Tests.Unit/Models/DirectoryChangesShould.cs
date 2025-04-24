namespace AStar.Dev.Database.Updater.Models;

public sealed class DirectoryChangesShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new DirectoryChanges()
           .ToString()
           .ShouldMatchApproved();
}
