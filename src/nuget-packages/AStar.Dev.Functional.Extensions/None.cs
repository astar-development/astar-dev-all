namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
public sealed class None
{
    /// <summary>
    /// </summary>
    public static None Value { get; } = new();

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static None<T> Of<T>() =>
        new();
}
