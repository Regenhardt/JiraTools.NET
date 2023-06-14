namespace JiraLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class JiraGraphService
{
    private const string GoogleChartUrl = "https://chart.apis.google.com/chart";
    public int MaxSummaryLength { get; set; } = 30;

    /// <summary>
    /// Build the graph starting with the given issues.
    /// </summary>
    /// <param name="jira">Already initialized instance to get ticket info from.</param>
    /// <param name="input">Options about what the graph should include.</param>
    /// <returns>A string containing the finished graph in Graphviz (gv/dot) format.</returns>
    public async Task<string> GetGraph(JiraSearch jira, Options input)
    {
        var graph = new List<string>();
        foreach (var issue in input.Issues)
        {
            graph.AddRange(await BuildGraphData(issue, jira, input.ExcludeLinks, input.ShowDirections, input.WalkDirections, input.Includes, input.IssueExcludes,
            input.IgnoreClosed, input.IncludeEpics, input.IncludeSubtasks, input.Traverse, input.WordWrap));
        }
        graph = CleanGraph(graph);

        var sb = new StringBuilder("digraph G {").AppendLine();
        
        sb.AppendLine($"\tnode [shape={input.NodeShape}];");
        sb.AppendLine("\trankdir=LR;");

        foreach (var line in graph)
        {
            sb.Append('\t');
            sb.AppendLine(line);
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private async Task<List<string>> BuildGraphData(string issue, JiraSearch jira, ISet<string> excludeLinks,
        ISet<string> showDirections, ISet<string> walkDirections, string? includes, ISet<string> issueExcludes,
        bool ignoreClosed, bool includeEpics, bool includeSubtasks, bool traverse, bool wordWrap)
    {
        var nodes = new HashSet<string>();
        var edges = new HashSet<string>();

        await TraverseIssue(issue, jira, excludeLinks, nodes, edges, showDirections, walkDirections, includes,
            issueExcludes, ignoreClosed, includeEpics, includeSubtasks, traverse, wordWrap, new HashSet<string>());

        var graph = nodes.ToList();

        graph.AddRange(edges);

        return graph;
    }

    private async Task TraverseIssue(string issue, JiraSearch jira, ISet<string> excludeLinks, ISet<string> nodes,
        ISet<string> edges, ICollection<string> showDirections, ISet<string> walkDirections, string? includes,
        ISet<string> issueExcludes,
        bool ignoreClosed, bool includeEpics, bool includeSubtasks, bool traverse, bool wordWrap, ISet<string> seenIssues)
    {
        // Process issue

        if (seenIssues.Contains(issue)) return;

        var issueInfo = await jira.GetIssue(issue);
        if (issueInfo == null || (ignoreClosed && issueInfo.Fields.Status.IsClosed())) return;

        var summary = wordWrap ? WrapText(issueInfo.Fields.Summary, MaxSummaryLength) : issueInfo.Fields.Summary;

        if (includes != null && !summary.Contains(includes)) return;

        if (issueExcludes.Any(exclude => summary.Contains(exclude)))
        {
            Console.WriteLine($"Skipping {issue} - explicitly excluded.");
            return;
        }

        nodes.Add($"\"{issueInfo.Key}\" [label=\"{issueInfo.Key}\n{summary}\", href=\"{jira.GetIssueUri(issueInfo.Key)}\"style=filled, fillcolor={issueInfo.GetColour()}];");
        seenIssues.Add(issue);

        // Brauche ich das?
        //foreach (var exclude in excludeLinks)
        //    if (summary.Contains(exclude))
        //        return;

        if (!includeEpics && issueInfo.Fields.IssueType.IsEpic()) return;

        if (!includeSubtasks && issueInfo.Fields.IssueType.IsSubtask()) return;

        // Process linked issues

        if (issueInfo.Fields.Links != null)
            foreach (var edge in issueInfo.Fields.Links.Where(link => link.Target.Contains(includes ?? string.Empty)))
            {
                if (ignoreClosed && (edge.OutwardIssue ?? edge.InwardIssue)!.Fields.Status.IsClosed())
                {
                    Console.WriteLine($"Skipping {edge.Target} - linked key is closed.");
                    continue;
                }

                if (showDirections.Contains(edge.Direction.ToString().ToLower()) && !excludeLinks.Contains(edge.LinkType))
                    edges.Add($"\"{issue}\" -> \"{edge.Target}\" [label=\"{edge.LinkType}\"];");

                if (walkDirections.Contains(edge.Direction.ToString().ToLower()) && (traverse || issue.Split('-')[0] == edge.Target.Split('-')[0]))
                    await TraverseIssue(edge.Target, jira, excludeLinks, nodes, edges, showDirections, walkDirections,
                        includes, issueExcludes, ignoreClosed, includeEpics, includeSubtasks, traverse, wordWrap, seenIssues);
            }
    }

    private static string WrapText(string text, int maxLength)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            if (currentLine.Length + word.Length + 1 > maxLength)
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear();
            }

            if (currentLine.Length > 0) currentLine.Append(" ");

            currentLine.Append(word);
        }

        if (currentLine.Length > 0) lines.Add(currentLine.ToString());

        return string.Join("\\n", lines);
    }

    private static List<string> CleanGraph(List<string> graph)
    {
        var filteredGraph = new List<string>();
        var uniqueNodes = new HashSet<string>();

        foreach (var line in graph)
            if (line.StartsWith("\"") && line.EndsWith("\""))
            {
                var node = line.Trim('"');
                if (!uniqueNodes.Contains(node))
                {
                    uniqueNodes.Add(node);
                    filteredGraph.Add(line);
                }
            }
            else
            {
                filteredGraph.Add(line);
            }

        return filteredGraph;
    }

    public async Task<byte[]> GetGraphAsPng(JiraSearch jira, Options input)
    {
        var graphvizSource = await GetGraph(jira, input);

        var chartUrl = $"{GoogleChartUrl}?cht=gv&chl={Uri.EscapeDataString(graphvizSource)}";
        chartUrl += $"&chls=transparent&chshape={input.NodeShape}";

        using var httpClient = new HttpClient();
        return await httpClient.GetByteArrayAsync(chartUrl);
    }
}
