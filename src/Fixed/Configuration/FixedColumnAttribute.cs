namespace JK.Fixed.Configuration;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class FixedColumnAttribute(int width) : Attribute
{
    public int Width { get; } = width;
    public FixedColumnAlignment Alignment { get; init; } = FixedColumnAlignment.Left;
    public int Order { get; init; }
    public FixedColumnOverflow OverflowMode { get; init; } = FixedColumnOverflow.Truncate;
    public char PaddingCharacter { get; init; } = ' ';
    public string StringFormat { get; init; }

    public FixedColumnOptions ToOptions()
        => new()
        {
            Alignment = Alignment,
            Order = Order,
            PaddingCharacter = PaddingCharacter,
            OverflowMode = OverflowMode,
            StringFormat = StringFormat,
            Width = Width
        };
}
