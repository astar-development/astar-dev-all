using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Models;

public record Directory(string Old, string New)
{
    public override string ToString() =>
        this.ToJson();
}
