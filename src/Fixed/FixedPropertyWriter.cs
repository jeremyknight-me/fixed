using JK.Fixed.Configuration;
using JK.Fixed.Exceptions;

namespace JK.Fixed;

internal sealed class FixedPropertyWriter
{
    private readonly FixedProperty property;

    public FixedPropertyWriter(FixedProperty fixedProperty)
    {
        this.property = fixedProperty;
    }

    public string Write(object item)
    {
        var value = this.GetObjectPropertyValue(item);
        var overflowHandledValue = this.HandleValueOverflow(value);
        var paddedValue = this.PadValue(overflowHandledValue);
        return paddedValue;
    }

    private string GetObjectPropertyValue(object item)
    {
        var propertyValue = this.property.PropertyInfo.GetValue(item, null);
        if (propertyValue is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(this.property.ColumnOptions.StringFormat))
        {
            return propertyValue.ToString();
        }

        var format = "{0:" + this.property.ColumnOptions.StringFormat.Trim() + "}";
        return string.Format(format, propertyValue);
    }

    private string HandleValueOverflow(string value)
    {
        var width = this.property.ColumnOptions.Width;
        if (value.Length <= width)
        {
            return value;
        }

        if (this.property.ColumnOptions.OverflowMode == FixedColumnOverflow.Throw)
        {
            var propertyName = this.property.PropertyInfo.Name;
            throw new FixedOverflowException(propertyName, width, value);
        }

        return value[..width];
    }

    private string PadValue(string value)
        => this.property.ColumnOptions.Alignment == FixedColumnAlignment.Left
            ? value.PadRight(this.property.ColumnOptions.Width, this.property.ColumnOptions.PaddingCharacter)
            : value.PadLeft(this.property.ColumnOptions.Width, this.property.ColumnOptions.PaddingCharacter);
}
