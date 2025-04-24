namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Option<T>
{
    /// <summary>
    /// </summary>
    /// <param name="_"></param>
    public static implicit operator Option<T>(None _)
        => new None<T>();

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Option<T>(T value)
        => new Some<T>(value);
}
