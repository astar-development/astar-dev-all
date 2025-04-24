using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateMaskAttribute" /> class
/// </summary>
internal class GenerateMaskAttribute : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(static context => PostInitializationOutput(context));

    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        GenerateMaskAttributeCreator.PostInitializationOutput(context);
}
