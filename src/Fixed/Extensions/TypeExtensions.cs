﻿using JK.Fixed.Configuration;

namespace JK.Fixed.Extensions;

internal static class TypeExtensions
{
    internal static FixedProperty[] ToFixedColumnProperties(this Type mappingType)
    {
        var columnAttributeType = typeof(FixedColumnAttribute);
        var properties =
            from p in mappingType.GetProperties()
            where Attribute.IsDefined(p, columnAttributeType)
            let attributes = p.GetCustomAttributes(columnAttributeType, true)
            select new FixedProperty
            {
                PropertyInfo = p,
                ColumnOptions = attributes.Cast<FixedColumnAttribute>()
                    .FirstOrDefault()?
                    .ToOptions()
            };
        return properties
            .OrderBy(x => x.ColumnOptions.Order)
            .ToArray();
    }
}
