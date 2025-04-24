using Asp.Versioning.Builder;
using AStar.Dev.Files.Api.Duplicates.Get.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.AllFiles.V1;

public sealed class GetAllFilesEndpoint(WebApplication app)  : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.AllFilesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.AllFilesEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/",
                   async ([AsParameters] GetDuplicatesQuery parameters,
                          FilesContext                      filesContext,
                          ILogger<GetAllFilesEndpoint>      logger,
                          CancellationToken                 cancellationToken) => await Handle(parameters, filesContext, logger, cancellationToken))
           .AddBasicProduces<IEnumerable<GetDuplicatesQueryResponse>>()
           .WithDescription("Get all files matching the specified search criteria.")
           .WithSummary("Get all files")
           .RequireAuthorization()
           .WithTags(EndpointConstants.AllFilesTag);
    }

    /// <summary>
    ///     This is NOT correct - just copied from the duplicates... TODO - implement or delete
    /// </summary>
    /// <param name="request"></param>
    /// <param name="filesContext"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private Task<IResult> Handle(GetDuplicatesQuery request, FilesContext filesContext, ILogger<GetAllFilesEndpoint> logger, CancellationToken cancellationToken)
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
