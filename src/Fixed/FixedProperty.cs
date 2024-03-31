using System.Reflection;
using JK.Fixed.Attributes;

namespace JK.Fixed;

public sealed class FixedProperty
{
    public PropertyInfo PropertyInfo { get; init; }
    public FixedColumnAttribute Attribute { get; init; }
}
