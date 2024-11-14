using JK.Fixed.Readers;

namespace JK.Fixed;

public static class FixedReader
{
    public static IEnumerable<T> Read<T>(IEnumerable<string> lines) where T : new()
    {
        var parser = new FixedColumnAttributeLineParser<T>();
        foreach (var line in lines)
        {
            yield return parser.Parse(line);
        }
    }
}
