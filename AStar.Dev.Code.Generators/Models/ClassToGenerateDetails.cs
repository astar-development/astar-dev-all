namespace AStar.Dev.Code.Generators.Models;

/// <summary>
///     The <see cref="ClassToGenerateDetails" /> class
/// </summary>
internal class ClassToGenerateDetails
{
    private readonly IList<PropertyDetails> propertyDetailsList = [];

    /// <summary>
    ///     Gets or sets the NamespaceName for the class having code generated
    /// </summary>
    public string NamespaceName { get; internal set; } = "NotSpecified";

    /// <summary>
    ///     Gets or sets the ClassName for the class having code generated
    /// </summary>
    public string ClassName { get; internal set; } = "NotSpecified";

    /// <summary>
    ///     Gets or sets the Property Details for the class having code generated
    /// </summary>
    public IEnumerable<PropertyDetails> PropertyDetails => propertyDetailsList;

    /// <summary>
    ///     Adds a new Property Details to the internal list
    /// </summary>
    public void AddPropertyDetails(PropertyDetails propertyDetails) =>
        propertyDetailsList.Add(propertyDetails);
}
