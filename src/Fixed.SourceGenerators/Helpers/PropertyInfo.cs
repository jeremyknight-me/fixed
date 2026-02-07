using JK.Fixed.Configuration;

namespace JK.Fixed.SourceGenerators.Helpers;

public record struct PropertyInfo(
    string Name,
    string Type,
    bool IsStatic,
    string Accessibility,
    int Width,
    string Alignment,
    int Order,
    string OverflowMode,
    char PaddingCharacter,
    string StringFormat);
