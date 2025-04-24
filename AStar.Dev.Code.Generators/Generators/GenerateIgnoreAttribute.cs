using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateIgnoreAttribute" /> class
/// </summary>
internal class GenerateIgnoreAttribute : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(static context => PostInitializationOutput(context));

    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        GenerateIgnoreAttributeCreator.PostInitializationOutput(context);
}
