namespace AStar.Dev.Web.Services;

public class SearchFilesServiceData
{
    public const string PREVIOUS = "previous";
    public const string NEXT     = "next";
    public       string StartingFolder { get; set; } = @"C:\Users\jason\OneDrive\Pictures";

    public int ItemsOrGroupsPerPage { get; set; } = 10;

    public int SearchType { get; set; }

    public int SortOrder { get; set; }

    public int TotalPages { get; set; }

    public IReadOnlyCollection<int> PagesForPagination { get; set; } = [];

    public bool AccordionItem1Visible { get; set; } = true;

    public bool AccordionItem3Visible { get; set; } = false;

    public string CurrentPage { get; set; } = "1";

    public int CurrentPageAsInt { get; set; } = 1;

    public bool RecursiveSearch { get; set; } = true;

    public IEnumerable<int> Pages { get; set; } = [];
}
