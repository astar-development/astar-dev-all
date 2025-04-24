using Microsoft.CodeAnalysis;

namespace AStar.Dev.Code.Generators.Generators;

/// <summary>
///     The <see cref="GenerateRedactAttribute" /> class
/// </summary>
internal class GenerateRedactAttribute : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context) =>
        context.RegisterPostInitializationOutput(static context => PostInitializationOutput(context));

    private static void PostInitializationOutput(IncrementalGeneratorPostInitializationContext context) =>
        GenerateRedactAttributeCreator.PostInitializationOutput(context);
}
