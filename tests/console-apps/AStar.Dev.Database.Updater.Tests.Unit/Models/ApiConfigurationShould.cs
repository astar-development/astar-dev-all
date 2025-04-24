namespace AStar.Dev.Database.Updater.Models;

public sealed class ApiConfigurationShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new ApiConfiguration()
           .ToString()
           .ShouldMatchApproved();
}
