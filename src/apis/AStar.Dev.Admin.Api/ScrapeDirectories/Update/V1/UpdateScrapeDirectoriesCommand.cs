using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.Update.V1;

/// <summary>
///     The UpdateScrapeDirectoriesCommand class contains the parameters required to Update the Scrape Directories.
/// </summary>
public sealed class UpdateScrapeDirectoriesCommand : IEndpointName
{
    /// <summary>
    ///     Gets or sets the RootDirectory.
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the BaseSaveDirectory.
    /// </summary>
    public string BaseSaveDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the BaseDirectory.
    /// </summary>
    public string BaseDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the BaseDirectoryFamous.
    /// </summary>
    public string BaseDirectoryFamous { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the SubDirectoryName.
    /// </summary>
    public string SubDirectoryName { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.ScrapeDirectoriesEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
