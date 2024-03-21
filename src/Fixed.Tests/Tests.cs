using JK.Fixed.Attributes;

namespace JK.Fixed.Tests;

public class Tests
{
    [Fact]
    public void AttributeConfig_Should_Read_Fixed_Text()
    {
        List<string> lines = ["FalseAB2024-03-201234 4.30"];
        var items = FixedReader.FromLines<MappedTester>(lines);
        var item = Assert.Single(items);
        Assert.False(item.One);
        Assert.Equal("AB", item.Two);
        Assert.Equal(DateTime.Parse("2024-03-20"), item.Three);
        Assert.Equal(1234, item.Four);
        Assert.Equal(4.3m, item.Five);
    }

    [Fact]
    public void AttributeConfig_Should_Write_Fixed_Text()
    {
        List<MappedTester> items = [
            new MappedTester
            {
                One = false,
                Two = "AB",
                Three = DateTime.Parse("2024-03-20"),
                Four = 1234,
                Five = 4.3m
            }
        ];

        var lines = FixedWriter.ToLines(items);
        var line = Assert.Single(lines);
        Assert.Equal("FalseAB2024-03-201234 4.30", line);
    }

    [Fact]
    public void FluentConfig_Should_Read_Fixed_Text()
    {
        Assert.Fail();
    }

    [Fact]
    public void FluentConfig_Should_Write_Fixed_Text()
    {
        Assert.Fail();
    }
}

public class MappedTester
{
    [FixedColumn(5)]
    public bool One { get; set; }

    [FixedColumn(2)]
    public string Two { get; set; }

    [FixedColumn(10, StringFormat = "yyyy-MM-dd")]
    public DateTime Three { get; set; }

    [FixedColumn(4, Alignment = FixedColumnAlignment.Right)]
    public int Four { get; set; }

    [FixedColumn(5, StringFormat = "N2", Alignment = FixedColumnAlignment.Right)]
    public decimal Five { get; set; }
}

public class NonMappedTester
{
    public string One { get; set; }
    public string Two { get; set; }
    public string Three { get; set; }
    public string Four { get; set; }
    public string Five { get; set; }
}
