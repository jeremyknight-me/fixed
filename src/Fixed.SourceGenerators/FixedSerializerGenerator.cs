using JK.Fixed.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace JK.Fixed.SourceGenerators;

[Generator]
public sealed class FixedSerializerGenerator : IIncrementalGenerator
{
    private const string generatorNamespace = "JK.Fixed.SourceGenerators";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context
            .RegisterPostInitializationOutput(i =>
            {
                i.AddEmbeddedAttributeDefinition();
                i.AddSource(
                    "FixedSerializableAttribute.g.cs",
                    SourceText.From(FixedSerializableAttributeFactory.SourceCode, Encoding.UTF8)
                );
            });

        IncrementalValuesProvider<ClassTypeInfo> classDeclaration = context.SyntaxProvider
            .ForAttributeWithMetadataName($"{generatorNamespace}.FixedSerializableAttribute",
                 predicate: static (s, _) => true,
                 transform: static (ctx, _) => GetClassInfo(ctx));

        context.RegisterSourceOutput(classDeclaration,
            static (spc, source) => Execute(spc, source));
    }

    private static ClassTypeInfo GetClassInfo(GeneratorAttributeSyntaxContext ctx)
    {
        var type = (INamedTypeSymbol)ctx.TargetSymbol;
        var classInfo = new ClassTypeInfo(type);
        return classInfo;
    }

    private static void Execute(SourceProductionContext context, ClassTypeInfo classInfo)
    {
        var sb = new StringBuilder();
        // Initialize class
        if (!string.IsNullOrEmpty(classInfo.Namespace))
        {
            sb.AppendLine("");
            sb.AppendLine($"namespace {classInfo.Namespace};");
            sb.AppendLine("");
        }

        var className = $"{classInfo.Name}FixedSerializer";

        sb.Append(
            $$"""
            public class {{className}}
            {
            """);

        sb.AppendLine("}");

        context.AddSource(
            $"{generatorNamespace}.{className}.g.cs",
            SourceText.From(sb.ToString(), Encoding.UTF8)
        );
    }
}
