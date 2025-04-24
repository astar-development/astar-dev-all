using Asp.Versioning.Builder;
using AStar.Dev.Files.Api.Duplicates.Get.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Duplicates.Count.V1;

public sealed class GetDuplicatesCountEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.DuplicatesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.DuplicatesCountEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/",
                   async ([AsParameters] GetDuplicatesCountQuery parameters,
                          FilesContext                           filesContext,
                          ILogger<GetDuplicatesEndpoint>         logger,
                          CancellationToken                      cancellationToken) => await Handle(parameters, filesContext, logger, cancellationToken))
           .AddBasicProduces<GetDuplicatesCountQueryResponse>()
           .WithDescription("Get all file duplicates matching the specified search criteria.")
           .WithSummary("Get all file duplicates")

            //.RequireAuthorization()
           .WithTags(EndpointConstants.DuplicatesTag);
    }

    private Task<IResult> Handle(GetDuplicatesCountQuery request, FilesContext filesContext, ILogger<GetDuplicatesEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting duplicate files for: {SearchParameters}", request.ToJson());

        int duplicatesCount = filesContext.Files
                                          .Include(f => f.FileAccessDetail)
                                          .FilterByDirectory(request.SearchFolder, request.Recursive)
                                          .ExcludeAllButImages(request.SearchType)
                                          .IncludeSoftDeleted(request.IncludeSoftDeleted)
                                          .ExcludeViewed(request.ExcludeViewed)
                                          .IncludeWhenContains(request.SearchText)
                                          .SortBy(request.SortOrder)
                                          .FilterDuplicates()
                                          .Count();

        logger.LogDebug("scrapeDirectories duplicate files count: {DuplicateFilesCount}", duplicatesCount);

        return Task.FromResult<IResult>(TypedResults.Ok(new GetDuplicatesCountQueryResponse(duplicatesCount)));
    }
}
