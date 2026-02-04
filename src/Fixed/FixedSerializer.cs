using System.Text;

namespace JK.Fixed;

public static class FixedSerializer
{
    public static IEnumerable<T> Deserialize<T>(IEnumerable<string> lines) where T : new()
    {
        FixedColumnAttributeLineParser<T> parser = new();
        foreach (var line in lines)
        {
            yield return parser.Parse(line);
        }
    }

    public static IEnumerable<string> Serialize<T>(IEnumerable<T> items)
    {
        FixedProperty[] fixedProperties = typeof(T).ToFixedColumnProperties();
        foreach (T item in items)
        {
            yield return BuildLine(item, fixedProperties);
        }
    }

    private static string BuildLine<T>(T item, FixedProperty[] fixedProperties)
    {
        StringBuilder sb = new();
        foreach (FixedProperty property in fixedProperties)
        {
            FixedPropertyWriter propertyWriter = new(property);
            sb.Append(propertyWriter.Write(item));
        }

        return sb.ToString();
    }
}
