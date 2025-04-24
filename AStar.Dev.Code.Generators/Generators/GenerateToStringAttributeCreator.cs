using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateToStringAttributeCreator" /> class
/// </summary>
internal static class GenerateToStringAttributeCreator
{
    /// <summary>
    ///     The PostInitializationOutput performs any required Post Initialization completion
    /// </summary>
    /// <param name="context">An instance of <see cref="IncrementalGeneratorPostInitializationContext" /> used within the method</param>
    public static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        context.AddSource($"{Constants.NamespaceName}.GenerateToStringAttribute.g.cs",
                          $$"""
                            namespace {{Constants.NamespaceName}};

                            /// <summary>
                            /// The GenerateToString attribute is intended to be used on classes to simplify creating, as you would expect, the overridden ToString() method.
                            /// <para>
                            /// There are additional attributes to allow for Redacting, Masking or Ignoring specific properties. Any public property without one of these attributes applied will be included, in full, in the ToString output.
                            /// </para>
                            /// </summary>
                            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
                            public sealed class GenerateToStringAttribute : System.Attribute { }
                            """);
}
