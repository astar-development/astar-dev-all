using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateRedactAttributeCreator" /> class
/// </summary>
internal static class GenerateRedactAttributeCreator
{
    /// <summary>
    ///     The PostInitializationOutput should be run after the main generation
    /// </summary>
    /// <param name="context">An instance of the <see cref="IncrementalGeneratorPostInitializationContext" /> to add the source to</param>
    public static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        context.AddSource($"{Constants.NamespaceName}.RedactAttribute.g.cs",
                          $$"""
                            namespace {{Constants.NamespaceName}};

                            /// <summary>
                            /// The Redact attribute is intended to mark a property that should be redacted in the overridden ToString().
                            /// <para>
                            /// Simply add this attribute to any property you want to see in the ToString output, with the value replaced by 'Redacted'.
                            /// </para>
                            /// </summary>
                            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false)]
                            public sealed class RedactAttribute : System.Attribute { }
                            """);
}
