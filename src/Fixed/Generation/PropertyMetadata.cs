using JK.Fixed.Configuration;
using Microsoft.CodeAnalysis;

namespace JK.Fixed.Generation;

internal sealed class PropertyMetadata
{
    public IPropertySymbol Symbol { get; init; }
    public string Name { get; init; }
    public string TypeName { get; init; }
    public int Width { get; init; }
    public FixedColumnAlignment Alignment { get; init; }
    public int Order { get; init; }
    public FixedColumnOverflow Overflow { get; init; }
    public char PaddingCharacter { get; init; }
    public string StringFormat { get; init; }
}
