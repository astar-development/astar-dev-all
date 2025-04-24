using System.ComponentModel.DataAnnotations;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Models;

public sealed class DirectoryChanges
{
    public const string SectionLocation = "DirectoryChanges";

    [Required]
    public Directory[] Directories { get; set; } = [];

    public override string ToString() =>
        this.ToJson();
}
