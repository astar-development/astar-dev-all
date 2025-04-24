// using AStar.Dev.Admin.Api.Client.Sdk.AdminApi;
// using AStar.Dev.Files.Api.Client.SDK.FilesApi;
// using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
// using AStar.Dev.Usage.Api.Client.SDK.UsageApi;
using AStar.Dev.Web.Fakes;

namespace AStar.Dev.Web.StartupConfiguration;

public static class Services
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // _ = services.AddOptions<FilesApiConfiguration>()
        //             .Bind(configuration.GetSection(FilesApiConfiguration.SectionLocation))
        //             .ValidateDataAnnotations()
        //             .ValidateOnStart();
        //
        // _ = services.AddOptions<AdminApiConfiguration>()
        //             .Bind(configuration.GetSection(AdminApiConfiguration.SectionLocation))
        //             .ValidateDataAnnotations()
        //             .ValidateOnStart();
        //
        // _ = services.AddOptions<ImagesApiConfiguration>()
        //             .Bind(configuration.GetSection(ImagesApiConfiguration.SectionLocation))
        //             .ValidateDataAnnotations()
        //             .ValidateOnStart();
        //
        // _ = services.AddOptions<UsageApiConfiguration>()
        //             .Bind(configuration.GetSection(UsageApiConfiguration.SectionLocation))
        //             .ValidateDataAnnotations()
        //             .ValidateOnStart();
        //
        // _ = services.AddSignalR();
        //
         services.AddApiClient<FilesApiClient, FilesApiConfiguration>();
         services.AddApiClient<AdminApiClient, AdminApiConfiguration>();
         services.AddApiClient<ImagesApiClient, ImagesApiConfiguration>();
         services.AddApiClient<UsageApiClient, UsageApiConfiguration>();

         _ = services.AddScoped(typeof(ILoggerAstar<>), typeof(AStarLogger<>)); // remove later
        return services;
    }
}
