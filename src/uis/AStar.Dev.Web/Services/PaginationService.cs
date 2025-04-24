namespace AStar.Dev.Web.Services;

public class PaginationService
{
    public IReadOnlyCollection<int> GetPaginationInformation(int totalPageCount, int currentPage)
    {
        if (totalPageCount == 0)
        {
            return [];
        }

        if (totalPageCount <= 10)
        {
            return (IReadOnlyCollection<int>)Enumerable.Range(1, totalPageCount);
        }

        const int requiredPageCount = 5;
        var       middleList        = new List<int>();

        if (currentPage > 5 && currentPage < totalPageCount - 3)
        {
            middleList.AddRange(Enumerable.Range(currentPage - 4, requiredPageCount * 2));
        }

        return Enumerable.Range(1, requiredPageCount)
                         .Union(middleList)
                         .Union(Enumerable.Range(totalPageCount - 4, requiredPageCount))
                         .Where(page => page <= totalPageCount)
                         .ToList();
    }
}
