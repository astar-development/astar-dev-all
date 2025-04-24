using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Net.Mime;
using AStar.Dev.Database.Updater.Models;
using AStar.Dev.Database.Updater.Services;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;

namespace AStar.Dev.Database.Updater;

[ExcludeFromCodeCoverage]
internal sealed class Program
{
    private static void Main(string[] args)
    {
        string applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

        IConfigurationRoot configuration = new ConfigurationBuilder()
                                          .AddJsonFile("appsettings.json")
                                          .AddUserSecrets<Program>()
                                          .Build();

        Logger logger = new LoggerConfiguration()
                       .ReadFrom.Configuration(configuration)
                       .CreateLogger();

        try
        {
            Log.Logger = logger;
            logger.Information("Starting {AppName}", applicationName);

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            _ = builder.Services.AddOptions<ApiConfiguration>()
                       .Bind(configuration.GetSection(ApiConfiguration.SectionLocation))
                       .ValidateOnStart();

            _ = builder.Services.AddOptions<DirectoryChanges>()
                       .Bind(configuration.GetSection(DirectoryChanges.SectionLocation))
                       .ValidateOnStart();

            _ = builder.Services.AddOptions<FilesApiConfiguration>()
                       .Bind(configuration.GetSection(FilesApiConfiguration.SectionLocation))
                       .ValidateDataAnnotations()
                       .ValidateOnStart();

            _ = builder.Services.AddHttpClient<FilesApiClient>()
                       .ConfigurePrimaryHttpMessageHandler(() =>
                                                           {
                                                               var handler = new HttpClientHandler();
                                                               handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };

                                                               return handler;
                                                           })
                       .ConfigureHttpClient((serviceProvider, client) =>
                                            {
                                                client.BaseAddress = serviceProvider
                                                                    .GetRequiredService<IOptions<FilesApiConfiguration>>().Value
                                                                    .BaseUrl;

                                                client.DefaultRequestHeaders.Accept.Add(
                                                                                        new(MediaTypeNames.Application
                                                                                                          .Json));
                                            });

            _ = builder.Services.AddSerilog(config => config.ReadFrom.Configuration(builder.Configuration))
                       .AddHostedService<UpdateDatabaseForAllFiles>();

            //.AddHostedService<DeleteMarkedFiles>()
            //.AddHostedService<MoveFiles>();

            using var context = new FilesContext(
                                                 new() { Value         = configuration.GetConnectionString("sqlServer")!, },
                                                 new() { EnableLogging = false, IncludeSensitiveData = false, });

            _ = builder.Services.AddSingleton<IFileSystem, FileSystem>()
                       .AddSingleton<FilesService>()
                       .AddSingleton<DatabaseUpdateService>()
                       .AddSingleton(_ => TimeProvider.System)
                       .AddSingleton(_ => context);

            IHost host = builder.Build();

            _ = context.Database.EnsureCreated();

            host.Run();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Fatal error occurred in {AppName}", typeof(Program).AssemblyQualifiedName);
        }
    }
}
