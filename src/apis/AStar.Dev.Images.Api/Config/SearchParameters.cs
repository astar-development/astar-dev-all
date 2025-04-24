using System.ComponentModel.DataAnnotations;
using System.Text;
using AStar.Dev.Images.Api.Models;
using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Images.Api.Config;

/// <summary>
/// </summary>
public sealed class SearchParameters
{
    /// <summary>
    /// </summary>
    [Required]
    public string SearchFolder { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public SearchType SearchType { get; set; }

    /// <summary>
    /// </summary>
    public bool RecursiveSubDirectories { get; set; } = true;

    /// <summary>
    /// </summary>
    [Required]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// </summary>
    [Required]
    public int ItemsPerPage { get; set; } = 10;

    /// <summary>
    /// </summary>
    [Refactor(1, 1, "Remove this and use the Max Image Size below")]
    [Range(50, 450, ErrorMessage = "Please specify a size between 50 and 450 pixels")]
    public int MaximumSizeOfThumbnail { get; set; } = 150;

    /// <summary>
    /// </summary>
    [Range(50, 999999, ErrorMessage = "Please specify an image size between 500 and 999999 (NOT recommended!) pixels")]
    public int MaximumSizeOfImage { get; set; } = 1500;

    /// <summary>
    /// </summary>
    [Required]
    public SortOrder SortOrder { get; set; } = SortOrder.SizeDescending;

    /// <summary>
    /// </summary>
    public bool IncludeDimensions { get; set; }

    /// <summary>
    /// </summary>
    public bool CountOnly { get; set; }

    /// <summary>
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        _ = sb.Append($"{nameof(SearchFolder)}={SearchFolder}");
        _ = sb.Append($"&{nameof(CurrentPage)}={CurrentPage}");
        _ = sb.Append($"&{nameof(ItemsPerPage)}={ItemsPerPage}");
        _ = sb.Append($"&{nameof(SearchType)}={SearchType}");
        _ = sb.Append($"&{nameof(RecursiveSubDirectories)}={RecursiveSubDirectories}");
        _ = sb.Append($"&{nameof(SortOrder)}={SortOrder}");
        _ = sb.Append($"&{nameof(MaximumSizeOfThumbnail)}={MaximumSizeOfThumbnail}");
        _ = sb.Append($"&{nameof(MaximumSizeOfImage)}={MaximumSizeOfImage}");
        _ = sb.Append($"&{nameof(IncludeDimensions)}={IncludeDimensions}");
        _ = sb.Append($"&{nameof(CountOnly)}={CountOnly}");

        return sb.ToString();
    }
}
