namespace JK.Fixed.Writers;

internal sealed class FixedLineWriter : FixedWriterBase
{
    public IEnumerable<string> Write<TMapping>(IEnumerable<TMapping> items)
    {
        this.SetFixedColumnProperties(typeof(TMapping));
        foreach (var item in items)
        {
            yield return this.BuildLine(item);
        }
    }
}
