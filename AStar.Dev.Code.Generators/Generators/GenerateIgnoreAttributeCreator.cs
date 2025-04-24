using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateIgnoreAttributeCreator" /> class
/// </summary>
internal static class GenerateIgnoreAttributeCreator
{
    /// <summary>
    ///     The PostInitializationOutput method should run after the core code generation
    /// </summary>
    /// <param name="context">An instance of <see cref="IncrementalGeneratorPostInitializationContext" /> to add the source to</param>
    public static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        context.AddSource($"{Constants.NamespaceName}.IgnoreAttribute.g.cs",
                          $$"""
                            namespace {{Constants.NamespaceName}};

                            /// <summary>
                            /// The Ignore attribute is intended to mark a property that should not be included in the overridden ToString().
                            /// <para>
                            /// Simply add this attribute to any property you do not want to see in the ToString output.
                            /// </para>
                            /// </summary>
                            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false)]
                            public sealed class IgnoreAttribute : System.Attribute { }
                            """);
}
