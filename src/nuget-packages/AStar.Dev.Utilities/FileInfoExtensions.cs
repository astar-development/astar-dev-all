using System.IO.Abstractions;

namespace AStar.Dev.Utilities;

/// <summary>
/// </summary>
public static class FileInfoExtensions
{
    /// <summary>
    ///     The IsImage method returns whether the <see cref="IFileInfo" /> represents a supported image type
    /// </summary>
    /// <param name="fileInfo">The <see cref="IFileInfo" /> being extended</param>
    /// <returns><c>true</c> if the <see cref="IFileInfo" /> represents a supported image type. Otherwise, <c>false</c></returns>
    public static bool IsImage(this IFileInfo fileInfo)
        => fileInfo.Name.IsImage();
}
