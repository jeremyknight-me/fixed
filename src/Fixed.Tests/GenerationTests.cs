namespace JK.Fixed.Tests;

public class GenerationTests : TestBase
{
    public override IEnumerable<SampleObject> Deserialize(IEnumerable<string> lines)
        => SampleObjectFixedSerializer.Deserialize(lines);

    public override IEnumerable<string> Serialize(IEnumerable<SampleObject> items)
        => SampleObjectFixedSerializer.Serialize(items);
}
