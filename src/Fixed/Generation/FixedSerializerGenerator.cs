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

        sb.AppendNamespace(ns);

        var serializerClassName = $"{name}FixedSerializer";

        sb.AppendLine($"public static class {serializerClassName}");
        sb.AppendOpenCurlyLine(); // start class

        PropertyMetadata[] props = GetSymbolProperties(typeSymbol);

        sb.AppendIndent();
        sb.AppendLine($"public static IEnumerable<{name}> Deserialize(IEnumerable<string> lines)");
        sb.AppendOpenCurlyLine(1); // start Deserialize

        sb.AppendIndent(2);
        sb.AppendLine("foreach (var line in lines)");
        sb.AppendOpenCurlyLine(2); // start foreach
        sb.AppendIndent(3);
        sb.AppendLine("ReadOnlySpan<char> span = line.AsSpan();");
        sb.AppendIndent(3);
        sb.AppendLine($"yield return new {name}");
        sb.AppendOpenCurlyLine(3);

        var position = 0;
        for (var i = 0; i < props.Length; i++)
        {
            PropertyMetadata p = props[i];
            var start = position;
            var width = p.Width;
            string assign;
            // determine parse expression by type
            var t = p.TypeName;
            if (t == "string")
            {
                assign = $"{p.Name} = span.Slice({start}, {width}).ToString(),";
            }
            else if (t == "bool" || t == "System.Boolean")
            {
                assign = $"{p.Name} = bool.Parse(span.Slice({start}, {width})),";
            }
            else if (t == "int" || t == "System.Int32")
            {
                assign = $"{p.Name} = int.Parse(span.Slice({start}, {width})),";
            }
            else if (t == "decimal" || t == "System.Decimal")
            {
                assign = $"{p.Name} = decimal.Parse(span.Slice({start}, {width})),";
            }
            else if (t == "DateTime" || t == "System.DateTime")
            {
                if (!string.IsNullOrEmpty(p.StringFormat))
                {
                    assign = $"{p.Name} = DateTime.ParseExact(span.Slice({start}, {width}), \"{p.StringFormat}\", null),";
                }
                else
                {
                    assign = $"{p.Name} = DateTime.Parse(span.Slice({start}, {width})),";
                }
            }
            else
            {
                // fallback to calling Parse on type
                assign = $"{p.Name} = {p.TypeName}.Parse(span.Slice({start}, {width})),";
            }

            sb.AppendIndent(4);
            sb.AppendLine(assign);

            position += width;
        }

        // close object initializer and terminate the yield statement
        sb.AppendIndent(3);
        sb.AppendLine("};");
        sb.AppendCloseCurlyLine(2); // end foreach
        sb.AppendCloseCurlyLine(1); // end Deserialize

        sb.AppendEmptyLine();

        sb.AppendIndent();
        sb.AppendLine($"public static IEnumerable<string> Serialize(IEnumerable<{name}> items)");
        sb.AppendOpenCurlyLine(1); // start Serialize
        sb.AppendIndent(2);
        sb.AppendLine("foreach (" + name + " item in items)");
        sb.AppendOpenCurlyLine(2);
        sb.AppendIndent(3);
        sb.AppendLine("yield return string.Empty");
        for (var _i = 0; _i < props.Length; _i++)
        {
            PropertyMetadata p = props[_i];
            sb.AppendIndent(4);
            var suffix = _i == props.Length - 1 ? ";" : string.Empty;
            sb.AppendLine($"+ Serialize{p.Name}(item.{p.Name}){suffix}");
        }
        sb.AppendCloseCurlyLine(2);
        sb.AppendCloseCurlyLine(1); // end Serialize

        sb.AppendEmptyLine();

        // generate per-property serializer helpers
        foreach (PropertyMetadata p in props)
        {
            var type = p.TypeName;
            sb.AppendIndent();
            sb.AppendLine($"private static string Serialize{p.Name}({type} value)");
            sb.AppendOpenCurlyLine(1);
            sb.AppendIndent(2);
            sb.AppendLine($"var width = {p.Width};");

            if (!string.IsNullOrEmpty(p.StringFormat))
            {
                sb.AppendIndent(2);
                sb.AppendLine($"var text = value.ToString(\"{p.StringFormat}\");");
            }
            else
            {
                sb.AppendIndent(2);
                sb.AppendLine($"var text = value.ToString();");
            }

            // overflow handling
            if (p.Overflow?.EndsWith("Throw") == true)
            {
                sb.AppendIndent(2);
                sb.AppendLine($"if (text.Length > width)");
                sb.AppendOpenCurlyLine(2);
                sb.AppendIndent(3);
                sb.AppendLine($"throw new JK.Fixed.Exceptions.FixedOverflowException(\"{p.Name}\", width, text);");
                sb.AppendCloseCurlyLine(2);
                sb.AppendEmptyLine();
            }
            else
            {
                sb.AppendIndent(2);
                sb.AppendLine($"if (text.Length > width)");
                sb.AppendOpenCurlyLine(2);
                sb.AppendIndent(3);
                sb.AppendLine($"text = text.Substring(0, width);");
                sb.AppendCloseCurlyLine(2);
                sb.AppendEmptyLine();
            }

            // padding
            if (p.Alignment?.EndsWith("Right") == true)
            {
                sb.AppendIndent(2);
                sb.AppendLine($"return text.PadLeft(width, '{p.PaddingCharacter}');");
            }
            else
            {
                sb.AppendIndent(2);
                sb.AppendLine($"return text.PadRight(width, '{p.PaddingCharacter}');");
            }

            sb.AppendCloseCurlyLine(1);
            sb.AppendEmptyLine();
        }

        sb.AppendCloseCurlyLine(); // end class

        context.AddSource(
            $"{generatorNamespace}.{serializerClassName}.g.cs",
            SourceText.From(sb.ToString(), Encoding.UTF8)
        );
    }

    private static PropertyMetadata[] GetSymbolProperties(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IPropertySymbol>()
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "JK.Fixed.Configuration.FixedColumnAttribute")
            })
            .Where(x => x.Attribute != null)
            .Select(x =>
            {
                AttributeData attr = x.Attribute;
                var width = (int)attr.ConstructorArguments[0].Value!;
                var named = attr.NamedArguments.ToDictionary(k => k.Key, k => k.Value);
                var alignment = named.TryGetValue("Alignment", out TypedConstant a) && a.Value != null
                    ? a.Value.ToString()
                    : "Left";
                var order = named.TryGetValue("Order", out TypedConstant o) && o.Value != null
                    ? (int)o.Value!
                    : 0;
                var overflow = named.TryGetValue("OverflowMode", out TypedConstant ov) && ov.Value != null
                    ? ov.Value.ToString()
                    : "Truncate";
                var padding = named.TryGetValue("PaddingCharacter", out TypedConstant pc) && pc.Value != null
                    ? (char)pc.Value!
                    : ' ';
                var format = named.TryGetValue("StringFormat", out TypedConstant sf) && sf.Value != null
                    ? sf.Value.ToString()
                    : null;
                return new PropertyMetadata
                {
                    Symbol = x.Property,
                    Name = x.Property.Name,
                    TypeName = x.Property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    Width = width,
                    Alignment = alignment,
                    Order = order,
                    Overflow = overflow,
                    PaddingCharacter = padding,
                    StringFormat = format
                };
            })
            .OrderBy(p => p.Order)
            .ThenBy(p => p.Symbol.Locations.FirstOrDefault()?.SourceSpan.Start ?? 0)
            .ToArray();
    }
}
