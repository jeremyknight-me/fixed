namespace JK.Fixed.Tests;

public class ReflectionTests : TestBase
{
    public override IEnumerable<SampleObject> Deserialize(IEnumerable<string> lines)
        => FixedSerializer.Deserialize<SampleObject>(lines);

    public override IEnumerable<string> Serialize(IEnumerable<SampleObject> items)
        => FixedSerializer.Serialize(items);
}
