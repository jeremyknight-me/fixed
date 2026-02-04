using JK.Fixed.Configuration;
using JK.Fixed.Exceptions;

namespace JK.Fixed;

internal sealed class FixedPropertyWriter
{
    private readonly FixedProperty _property;

    public FixedPropertyWriter(FixedProperty fixedProperty)
    {
        _property = fixedProperty;
    }

    public string Write(object item)
    {
        var value = GetObjectPropertyValue(item);
        var overflowHandledValue = HandleValueOverflow(value);
        var paddedValue = PadValue(overflowHandledValue);
        return paddedValue;
    }

    private string GetObjectPropertyValue(object item)
    {
        var propertyValue = _property.PropertyInfo.GetValue(item, null);
        if (propertyValue is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(_property.ColumnOptions.StringFormat))
        {
            return propertyValue.ToString();
        }

        var format = "{0:" + _property.ColumnOptions.StringFormat.Trim() + "}";
        return string.Format(format, propertyValue);
    }

    private string HandleValueOverflow(string value)
    {
        var width = _property.ColumnOptions.Width;
        if (value.Length <= width)
        {
            return value;
        }

        if (_property.ColumnOptions.OverflowMode == FixedColumnOverflow.Throw)
        {
            var propertyName = _property.PropertyInfo.Name;
            throw new FixedOverflowException(propertyName, width, value);
        }

        return value[..width];
    }

    private string PadValue(string value)
        => _property.ColumnOptions.Alignment == FixedColumnAlignment.Left
            ? value.PadRight(_property.ColumnOptions.Width, _property.ColumnOptions.PaddingCharacter)
            : value.PadLeft(_property.ColumnOptions.Width, _property.ColumnOptions.PaddingCharacter);
}
