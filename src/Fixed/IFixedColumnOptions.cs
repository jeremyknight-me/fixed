namespace JK.Fixed;

public interface IFixedColumnOptions
{
    int Width { get; }
    FixedColumnAlignment Alignment { get; }
    int Order { get; }
    FixedColumnOverflow OverflowMode { get; }
    char PaddingCharacter { get; }
    string StringFormat { get; }
}
