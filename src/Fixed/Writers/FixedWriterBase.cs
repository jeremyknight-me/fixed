using System.Text;

namespace JK.Fixed.Writers;

internal abstract class FixedWriterBase : FixedSerializerBase
{
    protected string BuildLine<T>(T item)
    {
        StringBuilder sb = new();
        foreach (var property in this.FixedProperties)
        {
            var propertyWriter = new FixedPropertyWriter(property);
            sb.Append(propertyWriter.Write(item));
        }

        return sb.ToString();
    }   
}
