namespace AStar.Dev.Web.Fakes;

internal class ApiUsageEvent
{
    public object ApiEndpoint { get ; set ; } = new();
    public object HttpMethod  { get ; set ; } = new();
    public long   ElapsedMilliseconds { get ; set ; }
    public int    StatusCode          { get ; set ; }
    public object Timestamp           { get ; set ; } = new();
}
