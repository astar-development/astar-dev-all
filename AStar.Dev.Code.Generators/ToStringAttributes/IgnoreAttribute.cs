namespace AStar.Dev.Code.Generators.ToStringAttributes;

/// <summary>
///     The Ignore attribute is intended to mark a property that should not be included in the overridden ToString()
///     <para>
///         Simply add this attribute to any property you do not want to see in the ToString output
///     </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreAttribute : Attribute
{
}
