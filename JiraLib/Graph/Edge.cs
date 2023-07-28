namespace JiraLib.Graph;

using JiraLib.Models;

public record Edge(JiraIssue Issue, IssueLink Link): IGraphElement
{
    public string GetGraphvizCode(bool wordWrap) => $"\"{Issue.Key}\" -> \"{Link.Target}\" [label=\"{Link.DirectedLinkName}\"];";
}
