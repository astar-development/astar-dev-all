namespace AStar.Dev.Web.Fakes;

public class ExcludedViewSettings
{
    public ExcludeViewed ExcludeViewed             { get ; set ; } = new();
    public int        ExcludeViewedPeriodInDays { get ; set ; }
}
