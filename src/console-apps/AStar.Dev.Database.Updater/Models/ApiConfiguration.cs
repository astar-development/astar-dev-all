using System.ComponentModel.DataAnnotations;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Models;

public sealed class ApiConfiguration
{
    public const string SectionLocation = "ApiConfiguration";

    [Required]
    public string[] Directories { get; set; } = [];

    [Required]
    public FilesApiConfiguration FilesApiConfiguration { get; set; } = new() { Scopes = [], };

    public override string ToString() =>
        this.ToJson();
}
