using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Models;

/// <summary>
///     The <see cref="PropertyDetails" /> class to enable strongly-type parameters
/// </summary>
internal class PropertyDetails
{
    /// <summary>
    ///     Gets or sets the Property Name
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Attributes of the property details
    /// </summary>
    public IEnumerable<AttributeData> Attributes { get; set; } = [];
}
