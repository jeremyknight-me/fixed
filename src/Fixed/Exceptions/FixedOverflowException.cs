namespace JK.Fixed.Exceptions;

public sealed class FixedOverflowException : Exception
{
    public FixedOverflowException(string propertyName, int width, string value)
        : base($"Property '{propertyName}' cannot be longer than '{width}'. Current value: {value}")
    {
    }
}
