using JK.Fixed.Writers;

namespace JK.Fixed;

public static class FixedWriter
{
    public static IEnumerable<string> ToLines<TMapping>(IEnumerable<TMapping> items)
    {
        FixedLineWriter writer = new();
        return writer.Write(items);
    }
}
