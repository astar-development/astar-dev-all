using System.IO.Abstractions;
using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Files.Api.StartupConfiguration;

/// <summary>
/// </summary>
public static class Services
{
    /// <summary>
    /// </summary>
    public static IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped(_ => new FilesContext(
                                                     new() { Value         = configuration.GetConnectionString("SqlServer")!, },
                                                     new() { EnableLogging = false, IncludeSensitiveData = false, }));

        _ = services.AddSingleton<IFileSystem, FileSystem>();

        ServiceProvider sp      = services.BuildServiceProvider();
        FilesContext    context = sp.GetRequiredService<FilesContext>();
        _ = context.Database.EnsureCreated();

        return services;
    }

    /// <summary>
    ///     The ConfigureDatabase method which does exactly that.
    /// </summary>
    /// <param name="services">
    ///     An instance of the <see cref="IServiceCollection" /> interface that will be used to configure the database context.
    /// </param>
    /// <param name="configuration">An instance of <see cref="IConfiguration" /> used to configure the database</param>
    /// <returns>
    ///     The original instance of the <see href="WebApplicationBuilder"></see> to facilitate method chaining (AKA fluent
    ///     configuration).
    /// </returns>
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddScoped(_ => CreateFilesContext(configuration.GetConnectionString("SqlServer")!));

        ServiceProvider sp      = services.BuildServiceProvider();
        FilesContext    context = sp.GetRequiredService<FilesContext>();
        _ = context.Database.EnsureCreated();

        return services;
    }

    private static FilesContext CreateFilesContext(string connectionString) =>
        new(
            new() { Value         = connectionString, },
            new() { EnableLogging = false, IncludeSensitiveData = false, }
           );
}
