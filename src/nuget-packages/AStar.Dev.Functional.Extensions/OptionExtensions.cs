namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="obj"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static Option<TResult> Map<T, TResult>(this Option<T> obj, Func<T, TResult> map)
        => obj is Some<T> some ? new Some<TResult>(map(some.Content)) : new None<TResult>();

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Option<T> Filter<T>(this Option<T> obj, Func<T, bool> predicate)
        => obj is Some<T> some && !predicate(some.Content) ? new None<T>() : obj;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="substitute"></param>
    /// <returns></returns>
    public static T Reduce<T>(this Option<T> obj, T substitute)
        => obj is Some<T> some ? some.Content : substitute;

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="substitute"></param>
    /// <returns></returns>
    public static T Reduce<T>(this Option<T> obj, Func<T> substitute)
        => obj is Some<T> some ? some.Content : substitute();
}
