using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Logging.Extensions;

namespace AStar.Dev.Web.Services;

/// <summary>
/// </summary>
/// <param name="searchFilesServiceData"></param>
/// <param name="paginationService"></param>
/// <param name="filesApiClient"></param>
/// <param name="logger"></param>
public class SearchFilesService(SearchFilesServiceData searchFilesServiceData, PaginationService paginationService, FilesApiClient filesApiClient, ILoggerAstar<SearchFilesService> logger)
{
    /// <summary>
    /// </summary>
    /// <param name="currentPage"></param>
    /// <param name="excludedViewSettings"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<SearchResults> GetFilesAsync(int currentPage, ExcludedViewSettings excludedViewSettings, CancellationToken cancellationToken)
    {
        SortOrder sortOrderAsEnum = searchFilesServiceData.SortOrder switch
                                    {
                                        0 => SortOrder.SizeDescending,
                                        1 => SortOrder.SizeAscending,
                                        2 => SortOrder.NameDescending,
                                        3 => SortOrder.NameAscending,
                                        _ => throw new ArgumentOutOfRangeException(nameof(searchFilesServiceData.SortOrder)),
                                    };

        SearchType searchTypeAsEnum = searchFilesServiceData.SearchType switch
                                      {
                                          0 => SearchType.Images,
                                          1 => SearchType.All,
                                          2 => SearchType.Duplicates,
                                          3 => SearchType.DuplicateImages,
                                          _ => throw new ArgumentOutOfRangeException(nameof(searchFilesServiceData.SearchType)),
                                      };

        var searchParameters = new SearchParameters
                               {
                                   SearchFolder = searchFilesServiceData.StartingFolder, Recursive = searchFilesServiceData.RecursiveSearch, SearchType = searchTypeAsEnum, SortOrder = sortOrderAsEnum,
                                   CurrentPage = currentPage,
                                   ItemsPerPage = searchFilesServiceData.ItemsOrGroupsPerPage,
                                   ExcludedViewSettings = new() { ExcludeViewed = excludedViewSettings.ExcludeViewed, ExcludeViewedPeriodInDays = excludedViewSettings.ExcludeViewedPeriodInDays, },
                               };

        logger.LogInformation("Searching for files in: {SearchFolder} - {SortOrder}, and of {SearchType} (Full Search Parameters: {SearchParameters})", searchParameters.SearchFolder, sortOrderAsEnum,
                              searchTypeAsEnum, searchParameters);

        IReadOnlyCollection<DuplicateGroup> fileGroups         = await filesApiClient.GetDuplicateFilesAsync(searchParameters);
        int                                 filesCount         = (await filesApiClient.GetDuplicateFilesCountAsync(searchParameters)).Count;
        var                                 totalPages         = (int)Math.Ceiling(filesCount / (decimal)searchFilesServiceData.ItemsOrGroupsPerPage);
        IReadOnlyCollection<int>            pagesForPagination = paginationService.GetPaginationInformation(totalPages, currentPage);

        var pages = Enumerable.Range(1, totalPages).ToList();

        return new (fileGroups, filesCount, totalPages, pagesForPagination, pages);
    }
}
