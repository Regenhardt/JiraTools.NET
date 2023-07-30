namespace JiraLib.Graph;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Models;

/// <summary>
/// A node in the graph.
/// </summary>
/// <param name="Issue">The issue this node represents.</param>
public record Node(JiraIssue Issue) : IGraphElement
{
    private const string SkyBlue = "#87CEEB";

    /// <summary>
    /// Maximum length the summary will be displayed with, in characters.
    /// </summary>
    public static int MaxSummaryLength { get; set; } = 30;

    /// <summary>
    /// An identifying key for this node. This is the Jira issue key.
    /// </summary>
    [Key] public string Key => Issue.Key;

    /// <summary>
    /// Get the colour this node should be displayed in.
    /// </summary>
    /// <returns>A colour as HTML string, either name or hex code including the #.</returns>
    public string GetColour() => Issue.Fields.Status.Name.ToLower() switch
    {
        "closed" => "green",
        "done" => "green",
        "fertig" => "green",
        "it fertig" => "green",
        "qm erfolgt" => "green",
        "für update bereit" => "green",
        "für update freigegeben" => "green",
        "geschlossen" => "green",
        "in qm-review" => SkyBlue,
        "in qm-test" => SkyBlue,
        "zu testen" => SkyBlue,
        "in progress" => "yellow",
        "in arbeit" => "yellow",
        "blockiert" => "red",
        _ => "white"
    };

    /// <summary>
    /// Creates a line of graphviz code to represent this node. Includes key, summary, and a link to the issue as href attribute, making it clickable in svg. Also comes with the colour included.
    /// </summary>
    /// <param name="wordWrap">Whether or not to word wrap the summary.</param>
    /// <returns></returns>
    public string GetGraphvizCode(bool wordWrap)
    {
        var summary = wordWrap ? WrapText(Issue.Fields.Summary) : Issue.Fields.Summary;
        summary = EscapeQuotes(summary);
        return
            $"\"{Key}\" [label=\"{Key}\\n{summary}\"; href=\"{Issue.Self.Scheme}://{Issue.Self.Host}/browse/{Key}\"; style=filled; fillcolor=\"{GetColour()}\";];";
    }

    private static string WrapText(string text)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > MaxSummaryLength)
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear();
            }

            if (currentLine.Length > 0) currentLine.Append(' ');

            currentLine.Append(word);
        }

        if (currentLine.Length > 0) lines.Add(currentLine.ToString());

        return string.Join("\\n", lines);
    }

    private static string EscapeQuotes(string text) => text.Replace("\"", "\\\"");
}
