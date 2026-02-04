namespace JK.Fixed.SourceGenerators;

internal static class FixedSerializableAttributeFactory
{
    internal const string SourceCode =
        """
        namespace JK.Fixed.SourceGenerators;
        [System.AttributeUsage(System.AttributeTargets.Class)]
        internal sealed class FixedSerializableAttribute : System.Attribute
        {
            public FixedSerializableAttribute() {}
        }
        """;
}
