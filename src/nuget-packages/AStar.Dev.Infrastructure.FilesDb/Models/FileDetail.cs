﻿using AStar.Dev.Technical.Debt.Reporting;
using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     The FileDetail class containing the current properties
/// </summary>
public sealed class FileDetail
{
    /// <summary>
    ///     The default constructor required by EF Core
    /// </summary>
    public FileDetail()
    {
    }

    /// <summary>
    ///     The copy constructor that allows for passing an instance of FileInfo to this class, simplifying consumer code
    /// </summary>
    /// <param name="fileInfo">
    ///     The instance of FileInfo to use
    /// </param>
    public FileDetail(FileInfo fileInfo)
    {
        FileName      = fileInfo.Name;
        DirectoryName = fileInfo.DirectoryName!;
        FileSize      = fileInfo.Length;
    }

    /// <summary>
    ///     Gets or sets The ID of the <see href="FileDetail"></see>. I know, shocking...
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public FileAccessDetail FileAccessDetail { get; set; } = new();

    /// <summary>
    ///     Gets or sets the file name. I know, shocking...
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the directory containing the file detail. I know, shocking...
    /// </summary>
    public string DirectoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets the full name of the file with the path combined
    /// </summary>
    public string FullNameWithPath => Path.Combine(DirectoryName, FileName);

    /// <summary>
    ///     Gets or sets the height of the image. I know, shocking...
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets or sets the width of the image. I know, shocking...
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets or sets the file size. I know, shocking...
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     Returns true when the file is of a supported image type
    /// </summary>
    [Refactor(1, 1, "The IsImage appears in multiple files. Refactor this issue.")]
    public bool IsImage { get; set; }

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>
    ///     This object serialized as a JSON object
    /// </returns>
    public override string ToString()
        => this.ToJson();
}
