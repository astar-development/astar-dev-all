using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Web.Services;

/// <summary>
/// </summary>
/// <param name="FileGroups"></param>
/// <param name="FilesCount"></param>
/// <param name="TotalPages"></param>
/// <param name="PagesForPagination"></param>
/// <param name="Pages"></param>
public sealed record SearchResults(
    IReadOnlyCollection<DuplicateGroup> FileGroups,
    int                                 FilesCount,
    int                                 TotalPages,
    IReadOnlyCollection<int>            PagesForPagination,
    List<int>                           Pages);
