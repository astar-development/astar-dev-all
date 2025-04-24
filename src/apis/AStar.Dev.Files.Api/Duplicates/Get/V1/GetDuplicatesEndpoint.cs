using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Duplicates.Get.V1;

public sealed class GetDuplicatesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.DuplicatesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.DuplicatesEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/",
                   async ([AsParameters] GetDuplicatesQuery parameters,
                          FilesContext                      filesContext,
                          ILogger<GetDuplicatesEndpoint>    logger,
                          CancellationToken                 cancellationToken) => await Handle(parameters, filesContext, logger, cancellationToken))
           .AddBasicProduces<IEnumerable<GetDuplicatesQueryResponse>>()
           .WithDescription("Get all file duplicates matching the specified search criteria.")
           .WithSummary("Get all file duplicates")
           .RequireAuthorization()
           .WithTags(EndpointConstants.DuplicatesTag);
    }

    private Task<IResult> Handle(GetDuplicatesQuery request, FilesContext filesContext, ILogger<GetDuplicatesEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting duplicate files for: {SearchParameters}", request.ToJson());

        var scrapeDirectories = filesContext.Files
                                            .Include(f => f.FileAccessDetail)
                                            .FilterByDirectory(request.SearchFolder, request.Recursive)
                                            .ExcludeAllButImages(request.SearchType)
                                            .IncludeSoftDeleted(request.IncludeSoftDeleted)
                                            .ExcludeViewed(request.ExcludeViewed)
                                            .IncludeWhenContains(request.SearchText)
                                            .SortBy(request.SortOrder)
                                            .FilterDuplicates()
                                            .SelectPage(request.CurrentPage, request.ItemsPerPage)
                                            .ToDictionary(grouping => grouping.Key);

        logger.LogDebug("scrapeDirectories duplicate files count: {DuplicateFilesCount}", scrapeDirectories.Count);

        var data = scrapeDirectories
                  .Select(scrapeDirectory => new GetDuplicatesQueryResponse(scrapeDirectory))
                  .ToList();

        logger.LogDebug("Returning duplicate files count: {DuplicateFilesCount}", data.Count);

        return Task.FromResult<IResult>(TypedResults.Ok(data));
    }
}
