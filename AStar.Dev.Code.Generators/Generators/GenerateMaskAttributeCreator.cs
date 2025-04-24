using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateMaskAttributeCreator" /> class
/// </summary>
internal static class GenerateMaskAttributeCreator
{
    /// <summary>
    ///     The PostInitializationOutput to run after the core code generation
    /// </summary>
    /// <param name="context">An instance of <see cref="IncrementalGeneratorPostInitializationContext" /> to add the content to</param>
    public static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        context.AddSource($"{Constants.NamespaceName}.MaskAttribute.g.cs",
                          $$"""
                            namespace {{Constants.NamespaceName}};

                            /// <summary>
                            /// The Mask attribute is intended to mark a property that should be masked in the overridden ToString().
                            /// <para>
                            /// Simply add this attribute to any property you want to see in the ToString output, with the applicable masking applied.
                            /// </para>
                            /// </summary>
                            [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false)]
                            public sealed class MaskAttribute : System.Attribute { }
                            """);
}
