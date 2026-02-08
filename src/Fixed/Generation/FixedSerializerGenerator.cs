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
            sb.AppendEmptyLine();
            sb.AppendLine($"namespace {ns};");
            sb.AppendEmptyLine();
        }

        var serializerClassName = $"{name}FixedSerializer";

        sb.AppendLine($"public static class {serializerClassName}");
        sb.AppendOpenCurlyLine(); // start class

        sb.AppendIndent();
        sb.AppendLine($"public static IEnumerable<{name}> Deserialize(IEnumerable<string> lines)");
        sb.AppendOpenCurlyLine(1); // start Deserialize

        sb.AppendIndent(2);
        sb.AppendLine("foreach (var line in lines)");
        sb.AppendOpenCurlyLine(2); // start foreach
        sb.AppendIndent(3);
        sb.AppendLine("yield return default;");
        sb.AppendCloseCurlyLine(2); // end foreach
        sb.AppendCloseCurlyLine(1); // end Deserialize

        sb.AppendEmptyLine();

        sb.AppendIndent();
        sb.AppendLine($"public static IEnumerable<string> Serialize(IEnumerable<{name}> items)");
        sb.AppendOpenCurlyLine(1); // start Serialize
        sb.AppendIndent(2);
        sb.AppendLine($"return [];");
        sb.AppendCloseCurlyLine(1); // end Serialize

        sb.AppendCloseCurlyLine(); // end class

        context.AddSource(
            $"{generatorNamespace}.{serializerClassName}.g.cs",
            SourceText.From(sb.ToString(), Encoding.UTF8)
        );
    }
}
