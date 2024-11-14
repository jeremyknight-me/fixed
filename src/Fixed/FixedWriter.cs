using System.Text;
using JK.Fixed.Extensions;
using JK.Fixed.Writers;

namespace JK.Fixed;

public static class FixedWriter
{
    public static IEnumerable<string> Write<T>(IEnumerable<T> items)
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
