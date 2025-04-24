namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sequence"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        => sequence.Where(predicate)
                .Select<T, Option<T>>(x => x)
                .DefaultIfEmpty(None.Value)
                .First();
}
