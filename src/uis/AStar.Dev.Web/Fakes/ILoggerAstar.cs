namespace AStar.Dev.Web.Fakes;

public interface ILoggerAstar<T> : ILogger<T>
{
    void LogPageView(string apiUsageName);
}
