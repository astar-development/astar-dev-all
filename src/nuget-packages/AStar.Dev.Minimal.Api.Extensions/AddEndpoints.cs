using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Minimal.Api.Extensions;

/// <summary>
///     The AddEndpoints class adds the AddApplicationEndpoints extension method to the <see cref="WebApplication" />
///     class
/// </summary>
public static class AddEndpoints
{
    /// <summary>
    ///     The AddApplicationEndpoints method uses reflection to add all the endpoints that implement the
    ///     <see cref="IEndpoint" /> interface
    ///     <para>
    ///         If you have other endpoints to add, please use the standard Framework approach
    ///     </para>
    /// </summary>
    /// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoints to</param>
    /// <param name="assembly"></param>
    public static void AddApplicationEndpoints(this WebApplication app, Assembly assembly)
    {
        IEnumerable<Type> endpoints = assembly.GetTypes().Where(type => typeof(IEndpoint).IsAssignableFrom(type)
                                                                        && type != typeof(IEndpoint) && type is { IsPublic: true, IsAbstract: false, });

        foreach (Type? endpoint in endpoints)
        {
            var instance = (IEndpoint)Activator.CreateInstance(endpoint, app)!;
            instance.AddEndpoint();
        }
    }
}
