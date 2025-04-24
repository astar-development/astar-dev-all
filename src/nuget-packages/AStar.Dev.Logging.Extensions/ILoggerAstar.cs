using Microsoft.Extensions.Logging;

namespace AStar.Dev.Logging.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILoggerAstar<out T> : ILogger<T>
{
    /// <summary>
    /// </summary>
    /// <param name="pageName"></param>
    void LogPageView(string pageName);
}
