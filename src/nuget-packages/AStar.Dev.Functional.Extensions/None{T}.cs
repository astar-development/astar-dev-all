namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class None<T> : Option<T>
{
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => "None";
}
