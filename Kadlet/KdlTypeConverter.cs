namespace Kadlet
{
    /// <summary>
    /// A delegate used for custom converters for strings with a type annotation.
    /// </summary>
    public delegate KdlValue KdlTypeConverter(string input, string type, KdlReaderOptions options);
}