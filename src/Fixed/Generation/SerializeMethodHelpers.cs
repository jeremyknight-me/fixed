using System.Text;
using JK.Fixed.Configuration;

namespace JK.Fixed.Generation;

internal static class SerializeMethodHelpers
{
    internal static void Generate(StringBuilder sb, PropertyMetadata[] props, string typeSymbolName)
    {
        GenerateSerializeMethod(sb, props, typeSymbolName);

        // generate per-property serializer helpers
        foreach (PropertyMetadata p in props)
        {
            sb.AppendEmptyLine();
            GenerateSerializePropertyMethod(sb, p);
        }
    }

    private static void GenerateSerializeMethod(StringBuilder sb, PropertyMetadata[] props, string typeSymbolName)
    {
        sb.AppendIndent();
        sb.AppendLine($"public static IEnumerable<string> Serialize(IEnumerable<{typeSymbolName}> items)");
        sb.AppendOpenCurlyLine(1); // start Serialize
        sb.AppendIndent(2);
        sb.AppendLine($"foreach ({typeSymbolName} item in items)");
        sb.AppendOpenCurlyLine(2);
        sb.AppendIndent(3);
        sb.AppendLine("yield return string.Empty");
        for (var i = 0; i < props.Length; i++)
        {
            PropertyMetadata p = props[i];
            sb.AppendIndent(4);
            var suffix = i == props.Length - 1 ? ";" : string.Empty;
            sb.AppendLine($"+ Serialize{p.Name}(item.{p.Name}){suffix}");
        }
        sb.AppendCloseCurlyLine(2);
        sb.AppendCloseCurlyLine(1); // end Serialize
    }

    private static void GenerateSerializePropertyMethod(StringBuilder sb, PropertyMetadata p)
    {
        var type = p.TypeName;
        sb.AppendIndent();
        sb.AppendLine($"private static string Serialize{p.Name}({type} value)");
        sb.AppendOpenCurlyLine(1);
        sb.AppendIndent(2);
        sb.AppendLine($"var width = {p.Width};");
        GenerateStringFormatHandling(sb, p);
        GenerateOverflowHandling(sb, p);
        GenerateAlignmentHandling(sb, p);
        sb.AppendCloseCurlyLine(1);
    }

    private static void GenerateStringFormatHandling(StringBuilder sb, PropertyMetadata p)
    {
        sb.AppendIndent(2);
        sb.Append("var text = value.ToString(");
        if (!string.IsNullOrEmpty(p.StringFormat))
        {
            sb.Append($"\"{p.StringFormat}\"");
        }

        sb.AppendLine(");");
    }

    private static void GenerateOverflowHandling(StringBuilder sb, PropertyMetadata p)
    {
        sb.AppendIndent(2);
        sb.AppendLine("if (text.Length > width)");
        sb.AppendOpenCurlyLine(2);
        sb.AppendIndent(3);
        sb.AppendLine(p.Overflow == FixedColumnOverflow.Throw
            ? $"throw new JK.Fixed.Exceptions.FixedOverflowException(\"{p.Name}\", width, text);"
            : "text = text.Substring(0, width);");
        sb.AppendCloseCurlyLine(2);
        sb.AppendEmptyLine();
    }

    private static void GenerateAlignmentHandling(StringBuilder sb, PropertyMetadata p)
    {
        sb.AppendIndent(2);
        sb.Append("return text.Pad");
        sb.Append(p.Alignment == FixedColumnAlignment.Right ? "Left" : "Right");
        sb.AppendLine($"(width, '{p.PaddingCharacter}');");
    }
}
