using AStar.Dev.Web.Fakes;
using AStar.Dev.Web.Services;
using Blazorise.LoadingIndicator;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Pages.Files;

public partial class DuplicateFiles : ComponentBase
{
    private const string                   Previous              = "previous";
    private const string                   Next                  = "next";
    private       bool                     accordionItem1Visible = true;
    private       bool                     accordionItem3Visible;
    private       bool                     atleastOneSearchHasBeenPerformed;
    private       string                   currentPage        = "1";
    private       int                      currentPageAsInt   = 1;
    private       ExcludeViewed                     excludeViewed      = new();
    private       LoadingIndicator         loadingIndicator   = new();
    private       IEnumerable<int>         pages              = [];
    private       IReadOnlyCollection<int> pagesForPagination = [];
    private       bool                     searchHasNoResults;
    private       bool                     searchHasResults;
    private       int                      totalPages;

    private IEnumerable<DuplicateGroup>? FileGroups { get; set; }

    [Inject]
    public required ILoggerAstar<DuplicateFiles> Logger { get; set; }

    [Inject]
    private SearchFilesService SearchFilesService { get; set; } = null!;

    [Inject]
    private SearchFilesServiceData SearchFilesServiceData { get; set; } = null!;

    private SearchParameters SearchParameters { get; } = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogPageView(nameof(DuplicateFiles));
        SearchParameters.SearchFolder = @"C:\Users\jason\OneDrive\Pictures";
        await base.OnInitializedAsync();
    }

    private async Task StartSearch()
        => await SearchForMatchingFiles();

    private async Task SearchForMatchingFiles()
    {
        searchHasResults = false;

        accordionItem1Visible = true;
        await loadingIndicator.Show();
        FileGroups                                                                                                                     = [];

        (IReadOnlyCollection<DuplicateGroup> fileGroups, int filesCount, int totalNumberOfPages, IReadOnlyCollection<int> paginationPages, List<int> pagesList) =
            await SearchFilesService.GetFilesAsync(currentPageAsInt, new () { ExcludeViewed = excludeViewed, }, CancellationToken.None);

        FileGroups            = fileGroups;
        totalPages            = totalNumberOfPages;
        pagesForPagination    = paginationPages;
        pages                 = pagesList;
        accordionItem1Visible = filesCount == 0;
        accordionItem3Visible = filesCount > 0;

        atleastOneSearchHasBeenPerformed = true;
        searchHasResults                 = filesCount > 0  && atleastOneSearchHasBeenPerformed;
        searchHasNoResults               = !searchHasResults;
        Logger.LogInformation("FilesApiClient fileCount: {FileCount}", filesCount);

        await loadingIndicator.Hide();
    }

    private async Task OnSelectedValueChanged(int value)
    {
        currentPageAsInt = value;
        currentPage      = value.ToString();

        await SetActivePage(currentPage);
    }

    private bool IsActivePage(string page)
        => currentPage == page;

    private bool IsPageNavigationDisabled(string navigation)
        => navigation.Equals(Previous)
            ? currentPage.Equals("1")
            : navigation.Equals(Next) && currentPage.Equals(SearchFilesServiceData.ItemsOrGroupsPerPage.ToString());

    private async Task PreviousPage()
    {
        currentPageAsInt = int.Parse(currentPage);

        if (currentPageAsInt > 1)
        {
            currentPage = (currentPageAsInt - 1).ToString();
        }

        await SetActivePage(currentPage);
    }

    private async Task NextPage()
    {
        currentPageAsInt = int.Parse(currentPage);

        if (currentPageAsInt < totalPages)
        {
            currentPage = (currentPageAsInt + 1).ToString();
        }

        await SetActivePage(currentPage);
    }

    private async Task SetActivePage(string page)
    {
        FileGroups       = [];
        currentPage      = page;
        currentPageAsInt = int.Parse(currentPage);

        await SearchForMatchingFiles();
    }
}
