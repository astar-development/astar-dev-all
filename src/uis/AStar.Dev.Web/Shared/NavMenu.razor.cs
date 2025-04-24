using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace AStar.Dev.Web.Shared;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public partial class NavMenu : IDisposable
{
    private string? currentUrl;
    private bool    isDisposed;

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            NavigationManager                 =  null!;
        }

        isDisposed = true;
    }

    protected override void OnInitialized()
    {
        currentUrl                        =  NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
        StateHasChanged();
    }
}
