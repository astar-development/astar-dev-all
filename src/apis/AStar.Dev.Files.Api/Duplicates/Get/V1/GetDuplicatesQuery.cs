using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Files.Api.Config;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Files.Api.Duplicates.Get.V1;

/// <summary>
/// </summary>
public sealed class GetDuplicatesQuery : IEndpointName
{
    /// <summary>
    /// </summary>
    [Required]
    public string SearchFolder { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    /// </summary>
    public bool ExcludeViewed { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeSoftDeleted { get; set; }

    /// <summary>
    ///     What was the purpose / thought behind this?
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// </summary>
    [Required]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// </summary>
    [Required]
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    /// </summary>
    [Refactor(1, 1, "Remove this and use MaximumSizeOfImage below")]
    [Range(50, 850, ErrorMessage = "Please specify a thumbnail size between 50 and 850 pixels.")]
    public int MaximumSizeOfThumbnail { get; set; } = 150;

    /// <summary>
    /// </summary>
    [Range(50, 999999, ErrorMessage = "Please specify an image size between 500 and 999999 (NOT recommended!) pixels.")]
    public int MaximumSizeOfImage { get; set; } = 1500;

    /// <summary>
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<SortOrder>))]
    public SortOrder SortOrder { get; set; }

    /// <summary>
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter<SearchType>))]
    public SearchType SearchType { get; set; } = SearchType.Duplicates;

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.DuplicatesEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
