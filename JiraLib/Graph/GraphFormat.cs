namespace JiraLib.Graph;

/// <summary>
/// Format the graph can be delivered in.
/// </summary>
public enum GraphFormat
{
    /// <summary>
    /// Portable Network Graphics. Will return an image in a base64 encoded string.
    /// </summary>
    Png,
    /// <summary>
    /// Scalable Vector Graphics. Will return the raw SVG code in a string. Can be embedded in HTML directly, saved as a file, or base64 encoded and used in an image tag.
    /// </summary>
    Svg,
    /// <summary>
    /// Graphviz DOT format. Will return the raw DOT code in a string starting with "digraph G {" and ending with "}".
    /// </summary>
    Dot
}