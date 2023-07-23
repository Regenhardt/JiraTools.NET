namespace JiraLib.Graph;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using JiraLib.Models;

public record Node(JiraIssue Issue) : IGraphElement
{
    private const string SkyBlue = "#87CEEB";

    public static int MaxSummaryLength { get; set; } = 30;

    [Key] public string Key => Issue.Key;

    public string GetColour() => Issue.Fields.Status.Name.ToLower() switch
    {
        "closed" => "green",
        "done" => "green",
        "fertig" => "green",
        "it fertig" => "green",
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

    public string GetGraphvizCode(Options input)
    {
        var summary = input.WordWrap ? WrapText(Issue.Fields.Summary) : Issue.Fields.Summary;
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
