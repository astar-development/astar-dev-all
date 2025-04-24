using System.IO.Abstractions;
using Asp.Versioning.Builder;
using AStar.Dev.Minimal.Api.Extensions;

namespace AStar.Dev.Files.Api.Directories.GetAll.V1;

public sealed class GetAllDirectoriesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi("Directories");

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup("/directories")
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/{rootDirectory}",
                   async (string rootDirectory, IFileSystem fileSystem, HttpContext context, ILogger<GetAllDirectoriesEndpoint> logger, CancellationToken cancellationToken) =>
                       await Handle(new(rootDirectory, context.User), fileSystem, logger, cancellationToken))
           .Produces<IEnumerable<string>>()
           .Produces(401)
           .Produces(500)
           .WithDescription("Get all directories - starting at the specified root directory")
           .WithSummary("Get all directories")

            //.RequireAuthorization()
           .WithTags("Directories");
    }

    private Task<IResult> Handle(GetAllDirectoriesQuery request, IFileSystem fileSystem, ILogger<GetAllDirectoriesEndpoint> logger, CancellationToken cancellationToken)
    {
        string rootDirectory = request.RootDirectory.Replace("%2F", "/");
        logger.LogDebug("Getting all directories starting from: {RootDirectory}", rootDirectory);

        IEnumerable<string> dirs = fileSystem.Directory.EnumerateDirectories(rootDirectory, "*", SearchOption.TopDirectoryOnly);

        logger.LogDebug("Returning all directories ({RootDirectory})", rootDirectory);

        return Task.FromResult<IResult>(TypedResults.Ok(dirs));
    }
}
