using JK.Fixed.Attributes;
using JK.Fixed.Exceptions;

namespace JK.Fixed;

internal abstract class FixedSerializerBase
{
    protected FixedProperty[] FixedProperties { get; set; } = [];

    protected void SetFixedColumnProperties(Type mappingType)
    {
        var properties = this.GetFixedColumns(mappingType);
        this.FixedProperties = properties.Length == 0
            ? throw new FixedNotFoundException()
            : properties.OrderBy(x => x.Attribute.Order).ToArray();
    }

    private FixedProperty[] GetFixedColumns(Type type)
    {
        var columnAttributeType = typeof(FixedColumnAttribute);
        var properties =
            from p in type.GetProperties()
            where Attribute.IsDefined(p, columnAttributeType)
            let attributes = p.GetCustomAttributes(columnAttributeType, true)
            select new FixedProperty
            {
                PropertyInfo = p,
                Attribute = attributes.Cast<FixedColumnAttribute>().FirstOrDefault()
            };
        return properties.ToArray();
    }
}
