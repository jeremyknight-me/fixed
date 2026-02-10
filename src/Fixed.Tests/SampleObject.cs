using JK.Fixed.Configuration;

namespace JK.Fixed.Tests;

[FixedSerializable]
public class SampleObject
{
    private const char paddingCharacter = '_';

    [FixedColumn(5, PaddingCharacter = paddingCharacter)]
    public bool One { get; set; }

    [FixedColumn(2, OverflowMode = FixedColumnOverflow.Throw, PaddingCharacter = paddingCharacter)]
    public string Two { get; set; }

    [FixedColumn(10, StringFormat = "yyyy-MM-dd", PaddingCharacter = paddingCharacter)]
    public DateTime Three { get; set; }

    [FixedColumn(4, Alignment = FixedColumnAlignment.Right, PaddingCharacter = paddingCharacter)]
    public int Four { get; set; }

    [FixedColumn(5, StringFormat = "N2", Alignment = FixedColumnAlignment.Right, PaddingCharacter = paddingCharacter)]
    public decimal Five { get; set; }
}
