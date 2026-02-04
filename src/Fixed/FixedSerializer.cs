using System.Text;

namespace JK.Fixed;

public static class FixedSerializer
{
    public static IEnumerable<T> Deserialize<T>(IEnumerable<string> lines) where T : new()
    {
        var parser = new FixedColumnAttributeLineParser<T>();
        foreach (var line in lines)
        {
            yield return parser.Parse(line);
        }
    }

    public static IEnumerable<string> Serialize<T>(IEnumerable<T> items)
    {
        var fixedProperties = typeof(T).ToFixedColumnProperties();
        foreach (var item in items)
        {
            yield return BuildLine(item, fixedProperties);
        }
    }

    private static string BuildLine<T>(T item, FixedProperty[] fixedProperties)
    {
        StringBuilder sb = new();
        foreach (var property in fixedProperties)
        {
            var propertyWriter = new FixedPropertyWriter(property);
            sb.Append(propertyWriter.Write(item));
        }

        return sb.ToString();
    }
}
