namespace JK.Fixed.Tests;

public abstract class TestBase
{
    public abstract IEnumerable<string> Serialize(IEnumerable<SampleObject> items);
    public abstract IEnumerable<SampleObject> Deserialize(IEnumerable<string> lines);

    [Fact]
    public void Should_Read_Fixed_Text()
    {
        List<string> lines = [
            "FalseAB2024-03-201234_4.30",
            "True_CD2025-03-205678_1.23"
        ];
        IEnumerable<SampleObject> items = Deserialize(lines);
        Assert.Collection(items,
            i =>
            {
                Assert.False(i.One);
                Assert.Equal("AB", i.Two);
                Assert.Equal(DateTime.Parse("2024-03-20"), i.Three);
                Assert.Equal(1234, i.Four);
                Assert.Equal(4.3m, i.Five);
            },
            i =>
            {
                Assert.True(i.One);
                Assert.Equal("CD", i.Two);
                Assert.Equal(DateTime.Parse("2025-03-20"), i.Three);
                Assert.Equal(5678, i.Four);
                Assert.Equal(1.23m, i.Five);
            });
    }

    [Fact]
    public void Should_Write_Fixed_Text()
    {
        List<SampleObject> items = [
            new SampleObject
            {
                One = false,
                Two = "AB",
                Three = DateTime.Parse("2024-03-20"),
                Four = 1234,
                Five = 4.3m
            },
            new SampleObject
            {
                One = true,
                Two = "CD",
                Three = DateTime.Parse("2025-03-20"),
                Four = 5678,
                Five = 1.23m
            }
        ];
        IEnumerable<string> lines = Serialize(items);
        Assert.Collection(lines,
            l => Assert.Equal("FalseAB2024-03-201234_4.30", l),
            l => Assert.Equal("True_CD2025-03-205678_1.23", l));
    }
}
