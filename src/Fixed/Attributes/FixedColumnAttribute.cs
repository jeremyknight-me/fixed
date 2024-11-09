namespace JK.Fixed.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class FixedColumnAttribute(int width) : Attribute, IFixedColumnOptions
{
    public int Width { get; } = width;
    public FixedColumnAlignment Alignment { get; init; } = FixedColumnAlignment.Left;
    public int Order { get; init; }
    public FixedColumnOverflow OverflowMode { get; init; } = FixedColumnOverflow.Truncate;
    public char PaddingCharacter { get; init; } = ' ';
    public string StringFormat { get; init; }

    public IFixedColumnOptions ToOptions()
        => new FixedColumnOptions
        {
            Alignment = this.Alignment,
            Order = this.Order,
            PaddingCharacter = this.PaddingCharacter,
            OverflowMode = this.OverflowMode,
            StringFormat = this.StringFormat,
            Width = this.Width
        };
}
