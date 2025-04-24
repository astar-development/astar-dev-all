using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Files.Api.Duplicates.Get.V1;

[Refactor(2, 4, "Unless I am mistaken, we're returning the Domain Model...")]
public sealed class GetDuplicatesQueryResponse(KeyValuePair<FileSize, IGrouping<FileSize, FileDetail>> scrapeDirectory)
{
    public long FileSize
    {
        get
        {
            try
            {
                return scrapeDirectory.Key.FileLength;
            }
            catch
            {
                return 0;
            }
        }
    }

    public IReadOnlyCollection<FileDetail> Files => scrapeDirectory.Value.OrderBy(d => d.DirectoryName).ToList();
}
