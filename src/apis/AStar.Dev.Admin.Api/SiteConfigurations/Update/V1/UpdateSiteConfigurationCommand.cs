using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SiteConfigurations.Update.V1;

/// <summary>
///     The <see cref="UpdateSiteConfigurationCommand" /> class contains the parameters required to Update the Site Configuration
///     <para>
///         The class implements the <see cref="IRequest{IResult}" /> as defined within MediatR
///     </para>
/// </summary>
public sealed class UpdateSiteConfigurationCommand : IEndpointName
{
    /// <summary>
    ///     Gets or sets the Base Url for the login and search
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Url
    /// </summary>
    public string LoginUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Email Address
    /// </summary>
    public string LoginEmailAddress { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Password. This password should be the encrypted password, not the clear-text password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.SiteConfigurationsEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
