using System.Reflection;

namespace JK.Fixed;

public sealed class FixedProperty
{
    public PropertyInfo PropertyInfo { get; init; }
    public IFixedColumnOptions ColumnOptions { get; init; }
}
