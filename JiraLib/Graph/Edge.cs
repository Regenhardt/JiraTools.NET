namespace JiraLib.Graph;

using JiraLib.Models;

public record Edge(JiraIssue Issue, IssueLink Link): IGraphElement
{
    public string GetGraphvizCode(Options input) => $"\"{Issue.Key}\" -> \"{Link.Target}\" [label=\"{Link.DirectedLinkName}\"];";
}
