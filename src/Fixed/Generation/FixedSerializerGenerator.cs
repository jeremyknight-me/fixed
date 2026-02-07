using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace JK.Fixed.Generation;

[Generator]
public sealed class FixedSerializerGenerator : IIncrementalGenerator
{
    private const string generatorNamespace = "JK.Fixed";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<INamedTypeSymbol> classDeclaration = context.SyntaxProvider
            .ForAttributeWithMetadataName($"{generatorNamespace}.Configuration.FixedSerializableAttribute",
                 predicate: static (s, _) => true,
                 transform: static (ctx, _) => GetSymbol(ctx));

        context.RegisterSourceOutput(classDeclaration,
            static (spc, source) => Execute(spc, source));
    }

    private static INamedTypeSymbol GetSymbol(GeneratorAttributeSyntaxContext ctx)
        => (INamedTypeSymbol)ctx.TargetSymbol;

    private static void Execute(SourceProductionContext context, INamedTypeSymbol typeSymbol)
    {
        var ns = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : typeSymbol.ContainingNamespace.ToString();
        var name = typeSymbol.Name;


        var sb = new StringBuilder();
        // Initialize class
        if (!string.IsNullOrEmpty(ns))
        {
            sb.AppendLine("");
            sb.AppendLine($"namespace {ns};");
            sb.AppendLine("");
        }

        var className = $"{name}FixedSerializer";

        sb.Append(
            $$"""
            public class {{className}}
            {
            }
            """);

        context.AddSource(
            $"{generatorNamespace}.{className}.g.cs",
            SourceText.From(sb.ToString(), Encoding.UTF8)
        );
    }
}

