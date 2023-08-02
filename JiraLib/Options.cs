namespace JiraLib;
using Graph;
using Models;

/// <summary>
/// Options for creating a graph from Jira issues.
/// </summary>
public class Options
{
    /// <summary>
    /// Username to use for authentication.
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// Password to use for authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// JSESSIONID or OAuth token to use for authentication.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Call jira without authentication. Only works for public jira data.
    /// </summary>
    public bool NoAuth { get; set; }

    /// <summary>
    /// URI of your jira instance, e.g. https://jira.example.com or https://example.atlassian.net.
    /// </summary>
    public string JiraUrl { get; set; } = "http://jira.example.com";

    /// <summary>
    /// Which file to save the resulting graph to. Empty to not save and just return the graph.
    /// </summary>
    public string ImageFile { get; set; } = string.Empty;

    /// <summary>
    /// Build via local dot install. Default is true. Set to false to use the Google Graphviz API, which might run slightly faster but only supports PNG.
    /// </summary>
    public bool Local { get; set; }

    /// <summary>
    /// Whether or not to include epics in the graph.
    /// Default is false until I make the epics subgraphs as right now they are just nodes.
    /// </summary>
    public bool IncludeEpics { get; set; } = false;

    /// <summary>
    /// Links not to show in the graph e.g. "is blocked by".
    /// </summary>
    public HashSet<string> ExcludeLinks { get; set; } = new();

    /// <summary>
    /// Whether or not to ignore closed issues.
    /// Default is false.
    /// </summary>
    public bool IgnoreClosed { get; set; }

    /// <summary>
    /// Whitelist for which issues to include in the graph. Null to not filter issues.
    /// Can be a partial key like "JI-" or "123", which will include any ticket with that part in its key.
    /// </summary>
    public string? Includes { get; set; }

    /// <summary>
    /// Keys of issues that should not be included in the graph.
    /// Can use partial keys like "JI-" or "123", which will exclude any ticket with that part in its key.
    /// </summary>
    public HashSet<string> IssueExcludes { get; set; } = new();

    /// <summary>
    /// Directions to show in the graph. Default is [Inward, Outward].
    /// </summary>
    public HashSet<LinkDirection> ShowDirections { get; set; } = new() { LinkDirection.Inward, LinkDirection.Outward };

    /// <summary>
    /// Directions to walk along while building the graph. Default is [inward, outward].
    /// If subtasks are included (<seealso cref="IncludeSubtasks"/>), "Outward" will be used from issue to subtask.
    /// </summary>
    public ISet<LinkDirection> WalkDirections { get; set; } = new HashSet<LinkDirection> { LinkDirection.Inward, LinkDirection.Outward };

    /// <summary>
    /// Whether or not to traverse to other projects.
    /// Default is yes.
    /// </summary>
    public bool Traverse { get; set; } = true;

    /// <summary>
    /// Whether or not to wrap long ticket summaries. Default is false.
    /// More recommended the bigger the graph gets.
    /// </summary>
    public bool WordWrap { get; set; }

    /// <summary>
    /// Query to find issues to include in the graph.
    /// </summary>
    public string? JqlQuery { get; set; }

    /// <summary>
    /// Which issues to definitely include in the graph. Links from these issues will be followed recursively.
    /// </summary>
    public List<string> Issues { get; set; } = new();

    /// <summary>
    /// Whether or not to include subtasks in the graph.
    /// Default value is false because who cares.
    /// </summary>
    public bool IncludeSubtasks { get; set; } = false;

    /// <summary>
    /// Which shape the nodes of the graph should have. See https://graphviz.org/doc/info/shapes.html for options.
    /// Default value is box.
    /// </summary>
    public string NodeShape { get; set; } = "box";

    /// <summary>
    /// Which format to deliver the graph in. Default is SVG.
    /// PNG will be returned base64 encoded.
    /// Setting <seealso cref="Local"/> to false will always return a PNG as Google only supports PNG.
    /// </summary>
    public GraphFormat OutputFormat { get; set; } = GraphFormat.Svg;
}