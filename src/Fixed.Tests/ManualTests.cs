namespace JK.Fixed.Tests;

// manually implement fixed width serializer for SampleObject
// meant as an example of what the generated code would look like
public class ManualTests
{
    [Fact]
    public void Manual_Should_Read_Fixed_Text()
    {
        List<string> lines = ["FalseAB2024-03-201234 4.30"];
        IEnumerable<SampleObject> items = Manual_SampleObjectFixedSerializer.Deserialize(lines);
        SampleObject item = Assert.Single(items);
        Assert.False(item.One);
        Assert.Equal("AB", item.Two);
        Assert.Equal(DateTime.Parse("2024-03-20"), item.Three);
        Assert.Equal(1234, item.Four);
        Assert.Equal(4.3m, item.Five);
    }

    [Fact]
    public void Manual_Should_Write_Fixed_Text()
    {
        List<SampleObject> items = [
            new SampleObject
            {
                One = false,
                Two = "AB",
                Three = DateTime.Parse("2024-03-20"),
                Four = 1234,
                Five = 4.3m
            }
        ];

        IEnumerable<string> lines = Manual_SampleObjectFixedSerializer.Serialize(items);
        var line = Assert.Single(lines);
        Assert.Equal("FalseAB2024-03-201234 4.30", line);
    }
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
                One = bool.Parse(span.Slice(0, 5)),
                Two = span.Slice(5, 2).ToString(),
                Three = DateTime.ParseExact(span.Slice(7, 10), "yyyy-MM-dd", null),
                Four = int.Parse(span.Slice(17, 4)),
                Five = decimal.Parse(span.Slice(21, 5))
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

        return text.PadRight(width, ' ');
    }

    private static string SerializeTwo(string value)
    {
        var width = 2;
        if (value.Length > width)
        {
            throw new JK.Fixed.Exceptions.FixedOverflowException("Two", width, value);
        }

        return value.PadRight(width, ' ');
    }

    private static string SerializeThree(DateTime value)
    {
        var width = 10;
        var text = value.ToString("yyyy-MM-dd");
        if (text.Length > width)
        {
            text = text.Substring(0, width);
        }
        return text.PadRight(width, ' ');
    }

    private static string SerializeFour(int value)
    {
        var width = 4;
        var text = value.ToString();
        return text.PadLeft(width, ' ');
    }

    private static string SerializeFive(decimal value)
    {
        var width = 5;
        var text = value.ToString("N2");
        return text.PadLeft(width, ' ');
    }
}
