﻿using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
///     The <see href="DuplicateGroup"></see> class
/// </summary>
public sealed class DuplicateGroup
{
    /// <summary>
    ///     Gets or sets the FileSize for the group
    /// </summary>
    public long FileSize { get; set; } = -1;

    /// <summary>
    ///     Gets the FileSize in a nicer, display-friendly, style
    /// </summary>
    public string FileSizeForDisplay
        => FileSize / 1024 / 1024 > 0
            ? (FileSize / 1024D / 1024D).ToString("N2") + " Mb"
            : (FileSize / 1024D).ToString("N2")         + " Kb";

    /// <summary>
    ///     Gets or sets the list of <see href="FileDetail"></see> objects
    /// </summary>
    public IList<FileDetail> Files { get; set; } = [];

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>This object serialized as a JSON object</returns>
    public override string ToString()
        => this.ToJson();
}
