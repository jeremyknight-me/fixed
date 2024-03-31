using JK.Fixed.Attributes;

namespace JK.Fixed.Exceptions;

public sealed class FixedNotFoundException : Exception
{
    private const string message = $"No '{nameof(FixedColumnAttribute)}' found for given type.";

    public FixedNotFoundException()
        : base(message)
    {
    }
}
