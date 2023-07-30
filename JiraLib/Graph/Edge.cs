namespace JiraLib.Graph;

using Models;

/// <summary>
/// Represents a link between two issues.
/// </summary>
public record Edge : IGraphElement
{
    private readonly string source;
    private readonly string target;
    private readonly string label;

    /// <summary>
    /// Creates a new edge from the given issue and link.
    /// </summary>
    /// <param name="issue">The issue this edge starts from.</param>
    /// <param name="link">The link this edge represents.</param>
    public Edge(MinimalIssue issue, IssueLink link)
        : this(issue, link.OutwardIssue ?? link.InwardIssue!, link.Type.Name)
    {
    }

    /// <summary>
    /// Creates a new edge between two issues with the given label.
    /// </summary>
    /// <param name="source">The issue this edge starts from.</param>
    /// <param name="target">The issue this edge ends at.</param>
    /// <param name="label">The label of this edge.</param>
    public Edge(MinimalIssue source, MinimalIssue target, string label)
    {
        this.source = source.Key;
        this.target = target.Key;
        this.label = label;
    }

    /// <summary>
    /// Returns the graphviz code for this edge including source, target and label.
    /// </summary>
    /// <param name="wordWrap">Not used here (yet).</param>
    /// <returns>String that can be used as part of graphviz DOT code.</returns>
    public string GetGraphvizCode(bool wordWrap) => $"\"{source}\" -> \"{target}\" [label=\"{label}\"];";
}
