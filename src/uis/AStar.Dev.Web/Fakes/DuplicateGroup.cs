namespace AStar.Dev.Web.Fakes;

public class DuplicateGroup
{
    public IEnumerable<FileDetail> Files              { get ; set ; } = [];
    public string                  FileSizeForDisplay { get ; set ; }=string.Empty;
}
