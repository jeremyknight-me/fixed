using JK.Fixed.Readers;

namespace JK.Fixed;

public static class FixedReader
{
    public static IEnumerable<T> FromLines<T>(IEnumerable<string> lines)
    {
        var reader = new FixedLineReader<T>();
        return reader.Read(lines);
    }
}
