namespace JK.Fixed.Tests;

// manually implement fixed width serializer for SampleObject
// meant as an example of what the generated code would look like
public class ManualTests : TestBase
{
    public override IEnumerable<SampleObject> Deserialize(IEnumerable<string> lines)
        => Manual_SampleObjectFixedSerializer.Deserialize(lines);

    public override IEnumerable<string> Serialize(IEnumerable<SampleObject> items)
        => Manual_SampleObjectFixedSerializer.Serialize(items);
}

file static class Manual_SampleObjectFixedSerializer
{
    public static IEnumerable<SampleObject> Deserialize(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            ReadOnlySpan<char> span = line.AsSpan();
            yield return new SampleObject
            {
                One = bool.Parse(span.Slice(0, 5).Trim('_')),
                Two = span.Slice(5, 2).ToString().Trim('_'),
                Three = DateTime.ParseExact(span.Slice(7, 10).Trim('_'), "yyyy-MM-dd", null),
                Four = int.Parse(span.Slice(17, 4).Trim('_')),
                Five = decimal.Parse(span.Slice(21, 5).Trim('_'))
            };
        }
    }

    public static IEnumerable<string> Serialize(IEnumerable<SampleObject> items)
    {
        foreach (SampleObject item in items)
        {
            yield return string.Empty
                + SerializeOne(item.One)
                + SerializeTwo(item.Two)
                + SerializeThree(item.Three)
                + SerializeFour(item.Four)
                + SerializeFive(item.Five);
        }
    }

    private static string SerializeOne(bool value)
    {
        var width = 5;
        var text = value.ToString();
        if (text.Length > width)
        {
            text = text.Substring(0, width);
        }

        return text.PadRight(width, '_');
    }

    private static string SerializeTwo(string value)
    {
        var width = 2;
        if (value.Length > width)
        {
            throw new Exceptions.FixedOverflowException("Two", width, value);
        }

        return value.PadRight(width, '_');
    }

    private static string SerializeThree(DateTime value)
    {
        var width = 10;
        var text = value.ToString("yyyy-MM-dd");
        if (text.Length > width)
        {
            text = text.Substring(0, width);
        }

        return text.PadRight(width, '_');
    }

    private static string SerializeFour(int value)
    {
        var width = 4;
        var text = value.ToString();
        if (text.Length > width)
        {
            text = text.Substring(0, width);
        }

        return text.PadLeft(width, '_');
    }

    private static string SerializeFive(decimal value)
    {
        var width = 5;
        var text = value.ToString("N2");
        if (text.Length > width)
        {
            text = text.Substring(0, width);
        }

        return text.PadLeft(width, '_');
    }
}
