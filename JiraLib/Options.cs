namespace JiraLib;

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
    /// JSESSIONID to use for authentication.
    /// </summary>
    public string? Cookie { get; set; }
    public bool NoAuth { get; set; }
    public string JiraUrl { get; set; } = "http://jira.example.com";
    public string ImageFile { get; set; } = "IssueGraph.png";

    /// <summary>
    /// Return dot file code instead of svg image.
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
    /// </summary>
    public string? Includes { get; set; }

    /// <summary>
    /// Keys of issues that should not be included in the graph.
    /// </summary>
    public HashSet<string> IssueExcludes { get; set; } = new();

    /// <summary>
    /// Directions to show in the graph. Default is [inward, outward].
    /// </summary>
    public HashSet<string> ShowDirections { get; set; } = new() { "inward", "outward" };

    /// <summary>
    /// Directions to walk along while building the graph. Default is [inward, outward].
    /// </summary>
    public HashSet<string> WalkDirections { get; set; } = new() { "inward", "outward" };

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
    /// Default value is true. Recommend false for bigger graphs.
    /// </summary>
    public bool IncludeSubtasks { get; set; } = true;

    /// <summary>
    /// Which shape the nodes of the graph should have. See https://graphviz.org/doc/info/shapes.html for options.
    /// Default value is box.
    /// </summary>
    public string NodeShape { get; set; } = "box";
}