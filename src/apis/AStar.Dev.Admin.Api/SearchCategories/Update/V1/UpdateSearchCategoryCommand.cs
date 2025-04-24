using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchCategoryCommand" /> class contains the parameters required to Update the Search Category
/// </summary>
public sealed class UpdateSearchCategoryCommand : IEndpointName
{
    /// <summary>
    ///     Gets or sets the Order of the search category - i.e. which category should be 1st, 2nd, etc.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    ///     Gets or sets the Name of the category.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Last Known Image Count.
    /// </summary>
    public int LastKnownImageCount { get; set; }

    /// <summary>
    ///     Gets or sets the Last Page Visited number.
    /// </summary>
    public int LastPageVisited { get; set; }

    /// <summary>
    ///     Gets or sets the Total Pages for the results.
    /// </summary>
    public int TotalPages { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.SearchCategoriesEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
