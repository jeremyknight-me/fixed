using JK.Fixed.Configuration;

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
        //return new EquatableArray<PropertyInfo>(type.GetMembers()
        //    .Select(m =>
        //    new PropertyInfo(
        //        Name: m.Name,
        //        Type: m is IPropertySymbol property ? property.Type.ToString() : string.Empty,
        //        IsStatic: m.IsStatic,
        //        IsPublic: m.DeclaredAccessibility == Accessibility.Public))
        //    .Where(p => p.Name is not null && !string.IsNullOrEmpty(p.Type))
        //    .ToArray());

        List<PropertyInfo> columnProperties = [];
        foreach (ISymbol member in type.GetMembers())
        {
            if (member.Name is null)
            {
                continue;
            }

            var name = member.Name;
            var typeName = member is IPropertySymbol propertySymbol
                ? propertySymbol.Type.ToString()
                : string.Empty;
            if (string.IsNullOrWhiteSpace(typeName))
            {
                continue;
            }

            AttributeData markerAttribute = GetMarkerAttribute(member);
            if (markerAttribute is null)
            {
                continue;
            }

            int? width = null;
            FixedColumnAlignment? alignment = null;
            int? order = null;
            FixedColumnOverflow? overflowMode = null;
            char? paddingChar = null;
            string? stringFormat = null;
            foreach (KeyValuePair<string, TypedConstant> namedArg in markerAttribute.NamedArguments)
            {
                switch (namedArg.Key)
                {
                    case "Alignment":
                        alignment = (FixedColumnAlignment)namedArg.Value.Value;
                        break;
                    case "Order":
                        order = (int)namedArg.Value.Value;
                        break;
                    case "OverflowMode":
                        overflowMode = (FixedColumnOverflow)namedArg.Value.Value;
                        break;
                    case "PaddingCharacter":
                        paddingChar = (char)namedArg.Value.Value;
                        break;
                    case "StringFormat":
                        stringFormat = namedArg.Value.Value as string;
                        break;
                    case "Width":
                        width = (int)namedArg.Value.Value;
                        break;
                }
            }

            PropertyInfo info = new(
                Name: name,
                Type: typeName,
                IsStatic: member.IsStatic,
                Accessibility: member.DeclaredAccessibility.ToString(),
                Width: width ?? 0,
                Alignment: (alignment ?? FixedColumnAlignment.Left).ToString(),
                Order: order ?? 0,
                OverflowMode: (overflowMode ?? FixedColumnOverflow.Truncate).ToString(),
                PaddingCharacter: paddingChar ?? ' ',
                StringFormat: stringFormat ?? string.Empty);
            columnProperties.Add(info);
        }

        return new EquatableArray<PropertyInfo>(columnProperties.ToArray());
    }

    private static AttributeData? GetMarkerAttribute(ISymbol property)
    {
        foreach (AttributeData attribute in property.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == "JK.Fixed.Configuration.FixedPropertyAttribute")
            {
                return attribute;
            }
        }

        return null;
    }
}
