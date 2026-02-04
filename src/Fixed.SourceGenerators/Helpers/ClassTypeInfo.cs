namespace JK.Fixed.SourceGenerators.Helpers;

internal record struct ClassTypeInfo
{
    public string Namespace { get; }
    public string Name { get; }
    public bool HasNameProperty { get; }
    public EquatableArray<PropertyInfo> Properties { get; }
    public bool IsRecord { get; }

    public ClassTypeInfo(ITypeSymbol type)
    {
        Namespace = type.ContainingNamespace.IsGlobalNamespace
            ? null
            : type.ContainingNamespace.ToString();
        Name = type.Name;
        HasNameProperty = type.GetMembers()
            .Any(m => m.Name == "Name"
                && m is IPropertySymbol property
                && property.Type.SpecialType == SpecialType.System_String);
        IsRecord = type.IsRecord;
        Properties = GetProperties(type);
    }

    private static EquatableArray<PropertyInfo> GetProperties(ITypeSymbol type)
    {
        return new EquatableArray<PropertyInfo>(type.GetMembers()
            .Select(m =>
            new PropertyInfo(
                Name: m.Name,
                Type: m is IPropertySymbol property ? property.Type.ToString() : string.Empty,
                IsStatic: m.IsStatic,
                IsPublic: m.DeclaredAccessibility == Accessibility.Public))
            .Where(p => p.Name is not null && !string.IsNullOrEmpty(p.Type))
            .ToArray());
    }
}
