using System.Reflection;

namespace JK.Fixed.Readers;

internal sealed class FixedLineReader<T> : FixedReaderBase
{
    public FixedLineReader()
    {
        this.SetFixedColumnProperties(typeof(T));
    }

    public IEnumerable<T> Read(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            yield return this.ParseLine(line, Activator.CreateInstance<T>());
        }
    }

    private T ParseLine(string line, T entity)
    {
        var linePosition = 0;
        foreach (var property in this.FixedProperties)
        {
            var columnValue = this.GetColumnStringValue(line, linePosition, property);
            var convertedValue = this.ConvertToPropertyType(property, columnValue);
            property.PropertyInfo.SetValue(entity, convertedValue, null);
            linePosition += property.Attribute.Width;
        }
        return entity;
    }

    private string GetColumnStringValue(string line, int linePosition, FixedProperty property)
    {
        var width = property.Attribute.Width;
        return linePosition + width > line.Length
            ? line.Substring(linePosition)
            : line.Substring(linePosition, width);
    }

    private object ConvertToPropertyType(FixedProperty property, string memberValue)
    {
        var type = property.PropertyInfo.PropertyType;
        var nullable = this.IsNullable(type);
        if (nullable && string.IsNullOrWhiteSpace(memberValue))
        {
            return null;
        }

        if (this.IsGenericNullable(type))
        {
            type = Nullable.GetUnderlyingType(type);
        }

        memberValue = memberValue.Trim(property.Attribute.PaddingCharacter);
        return type switch
        {
            var t when t == typeof(string) => memberValue,
            var t when this.IsParsable(t) => this.ParseToType(t, memberValue),
            _ => memberValue
        };
    }

    private bool IsNullable(Type type)
        => type switch
        {
            var t when !t.IsValueType => true, // ref-type
            var t when this.IsGenericNullable(t) => true, // Nullable<T>
            _ => false // value-type
        };

    private bool IsGenericNullable(Type type)
        => Nullable.GetUnderlyingType(type) != null;

    private bool IsParsable(Type type)
        => type.GetInterfaces()
            .Any(c => c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IParsable<>));

    public object ParseToType(Type type, string s)
    {
        var query =
                from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                let parameters = m.GetParameters()
                where
                    m.Name == "Parse"
                    && parameters.Length == 2
                    && parameters[0].ParameterType == typeof(string)
                    && parameters[1].ParameterType == typeof(IFormatProvider)
                select m;
        var parseMethodInfo = query.FirstOrDefault();
        return parseMethodInfo is null
            ? default
            : parseMethodInfo.Invoke(null, [s, null]);
        
    }
}
