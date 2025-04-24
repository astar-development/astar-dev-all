using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     The currently supported SortOrders
/// </summary>
[Refactor(1, 1, "I think this is now duplicated too")]
public enum SortOrder
{
    /// <summary>
    /// </summary>
    SizeDescending,

    /// <summary>
    /// </summary>
    SizeAscending,

    /// <summary>
    /// </summary>
    NameDescending,

    /// <summary>
    /// </summary>
    NameAscending,
}
