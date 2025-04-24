using AStar.Dev.Admin.Api.Client.Sdk.AdminApi;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
using AStar.Dev.Web.StartupConfiguration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using NSubstitute;

namespace AStar.Dev.Web.UI.StartupConfiguration;

[TestSubject(typeof(AddApiHttpClient))]
public sealed class AddApiHttpClientShould
{
    private readonly IServiceCollection services;

    public AddApiHttpClientShould()
    {
        ITokenAcquisition? x = Substitute.For<ITokenAcquisition>();
        services = new ServiceCollection();
        services.AddScoped<ITokenAcquisition>(_ => x);
    }

    [Fact]
    public void AddTheAdminApiClient()
    {
        services.AddApiClient<AdminApiClient, AdminApiConfiguration>();
        AdminApiClient? apiClient = services.BuildServiceProvider().GetService<AdminApiClient>();

        apiClient.ShouldNotBeNull();
    }

    [Fact]
    public void AddTheFilesApiClient()
    {
        services.AddApiClient<FilesApiClient, FilesApiConfiguration>();
        FilesApiClient? apiClient = services.BuildServiceProvider().GetService<FilesApiClient>();

        apiClient.ShouldNotBeNull();
    }

    [Fact]
    public void AddTheImagesApiClient()
    {
        services.AddApiClient<ImagesApiClient, ImagesApiConfiguration>();
        ImagesApiClient? apiClient = services.BuildServiceProvider().GetService<ImagesApiClient>();

        apiClient.ShouldNotBeNull();
    }
}
