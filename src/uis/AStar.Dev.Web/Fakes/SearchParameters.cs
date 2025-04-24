namespace AStar.Dev.Web.Fakes;

public class SearchParameters
{
    public string               SearchFolder         { get ; set ; } = string.Empty;
    public bool                 Recursive            { get ; set ; }
    public SearchType           SearchType           { get ; set ; }
    public SortOrder            SortOrder            { get ; set ; }
    public int                  CurrentPage          { get ; set ; }
    public int                  ItemsPerPage         { get ; set ; }
    public ExcludedViewSettings ExcludedViewSettings { get ; set ; } = new();
}
