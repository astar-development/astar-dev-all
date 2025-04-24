using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AStar.Dev.Web.Pages.Admin;

/// <summary>
///     Base class for UserClaims component.
///     Retrieves claims present in the ID Token issued by Azure AD.
/// </summary>
public class UserClaimsBase : ComponentBase
{
    // Defines list of claim types that will be displayed after successful sign-in.
    private readonly string[]           printClaims = ["name", "preferred_username", "tid", "oid",];
    protected        string             AuthMessage = "Not Authorized - default message";
    protected        IEnumerable<Claim> Claims      = [];

    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
        => await GetClaimsPrincipalData();

    /// <summary>
    ///     Retrieves user claims for the signed-in user.
    /// </summary>
    /// <returns></returns>
    private async Task GetClaimsPrincipalData()
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        ClaimsPrincipal user = authState.User;

        if (user.Identity is
            {
                IsAuthenticated: true,
            })
        {
            AuthMessage = $"{user.Identity.Name} is authenticated.";

            Claims = user.Claims; //.Where(x => printClaims.Contains(x.Type)); // The Where will, as you can guess, limit the results listed
        }
        else
        {
            AuthMessage = "The user is NOT authenticated.";
        }
    }
}
