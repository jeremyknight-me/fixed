using System.Reflection;
using JK.Fixed.Configuration;

namespace JK.Fixed.Reflection;

internal sealed class FixedProperty
{
    public PropertyInfo PropertyInfo { get; init; }
    public FixedColumnOptions ColumnOptions { get; init; }
}
