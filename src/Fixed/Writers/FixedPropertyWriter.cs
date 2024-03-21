using JK.Fixed.Exceptions;

namespace JK.Fixed.Writers;

internal sealed class FixedPropertyWriter(FixedProperty fixedProperty)
{
    private readonly FixedProperty property = fixedProperty;

    public string Write<T>(T item)
    {
        var value = this.GetObjectPropertyValue(item);
        var overflowHandledValue = this.HandleValueOverflow(value);
        var paddedValue = this.PadValue(overflowHandledValue);
        return paddedValue;
    }

    private string GetObjectPropertyValue<T>(T item)
    {
        var propertyValue = this.property.PropertyInfo.GetValue(item, null);
        if (propertyValue is null)
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(this.property.Attribute.StringFormat))
        {
            return propertyValue.ToString();
        }

        var format = "{0:" + this.property.Attribute.StringFormat.Trim() + "}";
        return string.Format(format, propertyValue);
    }

    private string HandleValueOverflow(string value)
    {
        var width = this.property.Attribute.Width;
        if (value.Length <= width)
        {
            return value;
        }

        if (this.property.Attribute.OverflowMode == FixedColumnOverflow.Throw)
        {
            var propertyName = this.property.PropertyInfo.Name;
            throw new FixedOverflowException(propertyName, width, value);
        }

        return value[..width];
    }

    private string PadValue(string value)
        => this.property.Attribute.Alignment == FixedColumnAlignment.Left
            ? value.PadRight(this.property.Attribute.Width, this.property.Attribute.PaddingCharacter)
            : value.PadLeft(this.property.Attribute.Width, this.property.Attribute.PaddingCharacter);
}
