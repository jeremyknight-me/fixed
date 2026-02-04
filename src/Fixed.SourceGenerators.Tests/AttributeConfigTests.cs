namespace JK.Fixed.SourceGenerators.Tests;

public class AttributeConfigTests
{
    [Fact]
    public void AttributeConfig_Should_Read_Fixed_Text()
    {
        SampleObjectFixedSerializer serializer = new();

        Assert.Fail();

        //List<string> lines = ["FalseAB2024-03-201234 4.30"];
        //IEnumerable<SampleObject> items = FixedSerializer.Deserialize<SampleObject>(lines);
        //SampleObject item = Assert.Single(items);
        //Assert.False(item.One);
        //Assert.Equal("AB", item.Two);
        //Assert.Equal(DateTime.Parse("2024-03-20"), item.Three);
        //Assert.Equal(1234, item.Four);
        //Assert.Equal(4.3m, item.Five);
    }

    [Fact]
    public void AttributeConfig_Should_Write_Fixed_Text()
    {
        SampleObjectFixedSerializer serializer = new();

        Assert.Fail();

        //List<SampleObject> items = [
        //    new SampleObject
        //    {
        //        One = false,
        //        Two = "AB",
        //        Three = DateTime.Parse("2024-03-20"),
        //        Four = 1234,
        //        Five = 4.3m
        //    }
        //];

        //IEnumerable<string> lines = FixedSerializer.Serialize(items);
        //var line = Assert.Single(lines);
        //Assert.Equal("FalseAB2024-03-201234 4.30", line);
    }
}
