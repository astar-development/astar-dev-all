namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
public static class Option
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Option<T> Optional<T>(this T obj)
        => new Some<T>(obj);
}
