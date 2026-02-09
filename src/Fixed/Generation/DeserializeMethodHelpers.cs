using System.Text;

namespace JK.Fixed.Generation;

internal static class DeserializeMethodHelpers
{
    internal static void Generate(StringBuilder sb, PropertyMetadata[] props, string typeSymbolName)
    {
        sb.AppendIndent();
        sb.AppendLine($"public static IEnumerable<{typeSymbolName}> Deserialize(IEnumerable<string> lines)");
        sb.AppendOpenCurlyLine(1); // start Deserialize

        sb.AppendIndent(2);
        sb.AppendLine("foreach (var line in lines)");
        sb.AppendOpenCurlyLine(2); // start foreach
        sb.AppendIndent(3);
        sb.AppendLine("ReadOnlySpan<char> span = line.AsSpan();");
        sb.AppendIndent(3);
        sb.AppendLine($"yield return new {typeSymbolName}");
        sb.AppendOpenCurlyLine(3);

        var start = 0;
        foreach (PropertyMetadata p in props)
        {
            var width = p.Width;
            var assign = p.TypeName switch
            {
                "string" => $"{p.Name} = span.Slice({start}, {width}).ToString().Trim(),",
                "bool" or "System.Boolean" => $"{p.Name} = bool.Parse(span.Slice({start}, {width})),",
                "int" or "System.Int32" => $"{p.Name} = int.Parse(span.Slice({start}, {width})),",
                "decimal" or "System.Decimal" => $"{p.Name} = decimal.Parse(span.Slice({start}, {width})),",
                "DateTime" or "System.DateTime" => string.IsNullOrEmpty(p.StringFormat)
                    ? $"{p.Name} = DateTime.Parse(span.Slice({start}, {width})),"
                    : $"{p.Name} = DateTime.ParseExact(span.Slice({start}, {width}), \"{p.StringFormat}\", null),",
                _ => $"{p.Name} = {p.TypeName}.Parse(span.Slice({start}, {width}))," // fallback to calling Parse on type
            };
            sb.AppendIndent(4);
            sb.AppendLine(assign);

            start += width;
        }

        // close object initializer and terminate the yield statement
        sb.AppendIndent(3);
        sb.AppendLine("};");
        sb.AppendCloseCurlyLine(2); // end foreach
        sb.AppendCloseCurlyLine(1); // end Deserialize
    }
}
