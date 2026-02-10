using System.Reflection;

namespace JK.Fixed.Reflection;

internal sealed class FixedColumnAttributeLineParser<T>
    where T : new()
{
    private readonly FixedProperty[] _fixedProperties;

    public FixedColumnAttributeLineParser()
    {
        _fixedProperties = typeof(T).ToFixedColumnProperties();
    }

    internal T Parse(string line)
    {
        T entity = new();
        var linePosition = 0;
        foreach (FixedProperty property in _fixedProperties)
        {
            var columnValue = GetColumnStringValue(line, linePosition, property);
            var convertedValue = ConvertToPropertyType(property, columnValue);
            property.PropertyInfo.SetValue(entity, convertedValue, null);
            linePosition += property.ColumnOptions.Width;
        }
        return entity;
    }

    private string GetColumnStringValue(string line, int linePosition, FixedProperty property)
    {
        var width = property.ColumnOptions.Width;
        return linePosition + width > line.Length
            ? line.Substring(linePosition)
            : line.Substring(linePosition, width);
    }

    private object ConvertToPropertyType(FixedProperty property, string memberValue)
    {
        Type type = property.PropertyInfo.PropertyType;
        var nullable = IsNullable(type);
        if (nullable && string.IsNullOrWhiteSpace(memberValue))
        {
            return null;
        }

        if (IsGenericNullable(type))
        {
            type = Nullable.GetUnderlyingType(type);
        }

        memberValue = memberValue.Trim(property.ColumnOptions.PaddingCharacter);
        if (type == typeof(string))
        {
            return memberValue.Trim();
        }

        return ParseToType(type, memberValue);
    }

    private bool IsNullable(Type type)
        => type switch
        {
            Type t when !t.IsValueType => true, // ref-type
            Type t when IsGenericNullable(t) => true, // Nullable<T>
            _ => false // value-type
        };

    private bool IsGenericNullable(Type type)
        => Nullable.GetUnderlyingType(type) != null;


    private object ParseToType(Type type, string s)
    {
        MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);

        // Look for Parse(string, IFormatProvider)
        MethodInfo parseWithProvider = (
            from m in methods
            let parameters = m.GetParameters()
            where
                m.Name == "Parse"
                && parameters.Length == 2
                && parameters[0].ParameterType == typeof(string)
                && parameters[1].ParameterType == typeof(IFormatProvider)
            select m)
            .FirstOrDefault();
        if (parseWithProvider != null)
        {
            return parseWithProvider.Invoke(null, [s, System.Globalization.CultureInfo.InvariantCulture]);
        }

        // Fallback to Parse(string)
        MethodInfo parse = (
            from m in methods
            let parameters = m.GetParameters()
            where
                m.Name == "Parse"
                && parameters.Length == 1
                && parameters[0].ParameterType == typeof(string)
            select m)
            .FirstOrDefault();

        return parse is null
            ? default
            : parse.Invoke(null, [s]);
    }
}
