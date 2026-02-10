using JK.Fixed.Configuration;
using Microsoft.CodeAnalysis;

namespace JK.Fixed.Generation;

internal static class NamedTypeSymbolExtensions
{
    internal static PropertyMetadata[] GetPropertyMetadata(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IPropertySymbol>()
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "JK.Fixed.Configuration.FixedColumnAttribute")
            })
            .Where(x => x.Attribute != null)
            .Select(x =>
            {
                AttributeData attr = x.Attribute;
                var named = attr.NamedArguments.ToDictionary(k => k.Key, k => k.Value);
                return new PropertyMetadata
                {
                    Symbol = x.Property,
                    Name = x.Property.Name,
                    TypeName = x.Property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    Width = (int)attr.ConstructorArguments[0].Value!,
                    Alignment = named.GetAlignment(),
                    Order = named.GetIntByName("Order"),
                    Overflow = named.GetOverflow(),
                    PaddingCharacter = named.GetCharByName("PaddingCharacter"),
                    StringFormat = named.GetStringByName("StringFormat")
                };
            })
            .OrderBy(p => p.Order)
            .ThenBy(p => p.Symbol.Locations.FirstOrDefault()?.SourceSpan.Start ?? 0)
            .ToArray();
    }

    private static FixedColumnAlignment GetAlignment(this Dictionary<string, TypedConstant> named)
        => named.TryGetValue("Alignment", out TypedConstant a) && a.Value != null
            ? (FixedColumnAlignment)a.Value
            : FixedColumnAlignment.Left;

    private static FixedColumnOverflow GetOverflow(this Dictionary<string, TypedConstant> named)
        => named.TryGetValue("OverflowMode", out TypedConstant ov) && ov.Value != null
            ? (FixedColumnOverflow)ov.Value
            : FixedColumnOverflow.Truncate;

    private static char GetCharByName(this Dictionary<string, TypedConstant> named, string name, char defaultValue = ' ')
        => named.TryGetValue(name, out TypedConstant pc) && pc.Value != null
            ? Convert.ToChar(pc.Value)
            : defaultValue;

    private static int GetIntByName(this Dictionary<string, TypedConstant> named, string name, int defaultValue = 0)
        => named.TryGetValue(name, out TypedConstant o) && o.Value != null
            ? (int)o.Value
            : defaultValue;

    private static string GetStringByName(this Dictionary<string, TypedConstant> named, string name, string defaultValue = null)
        => named.TryGetValue(name, out TypedConstant sf) && sf.Value != null
            ? sf.Value.ToString()
            : defaultValue;
}
