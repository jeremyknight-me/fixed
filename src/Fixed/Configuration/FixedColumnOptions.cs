namespace JK.Fixed.Configuration;

public sealed class FixedColumnOptions
{
    public int Width { get; init; }
    public FixedColumnAlignment Alignment { get; init; } = FixedColumnAlignment.Left;
    public int Order { get; init; }
    public FixedColumnOverflow OverflowMode { get; init; } = FixedColumnOverflow.Truncate;
    public char PaddingCharacter { get; init; } = ' ';
    public string StringFormat { get; init; }
}
