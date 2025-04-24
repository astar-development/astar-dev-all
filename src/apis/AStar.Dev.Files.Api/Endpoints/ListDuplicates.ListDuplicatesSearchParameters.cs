﻿using System.ComponentModel.DataAnnotations;
using AStar.Dev.Files.Api.Config;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Technical.Debt.Reporting;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints;

/// <summary>
/// </summary>
public sealed class ListDuplicatesSearchParameters
{
    /// <summary>
    /// </summary>
    [Required]
    public string SearchFolder { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    /// </summary>
    public bool ExcludeViewed { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeSoftDeleted { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    public string? SearchText { get; set; }

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
    [Refactor(1, 1, "Remove this and use MaximumSizeOfImage below - also, seems this and other properties are not used")]
    [Range(50, 850, ErrorMessage = "Please specify a thumbnail size between 50 and 850 pixels.")]
    public int MaximumSizeOfThumbnail { get; set; } = 150;

    /// <summary>
    /// </summary>
    [Range(50, 999999, ErrorMessage = "Please specify an image size between 500 and 999999 (NOT recommended!) pixels.")]
    public int MaximumSizeOfImage { get; set; } = 1500;

    /// <summary>
    /// </summary>
    [Required]
    public SortOrder SortOrder { get; set; }

    /// <summary>
    /// </summary>
    public SearchType SearchType { get; } = SearchType.Duplicates;

    /// <summary>
    /// </summary>
    public override string ToString() =>
        this.ToJson();
}
