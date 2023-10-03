namespace JiraLib.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph;
using Models;

/// <summary>
/// Service to build a graph from Jira issues.
/// </summary>
public class JiraGraphService
{
    private const string GoogleChartUrl = "https://chart.apis.google.com/chart";

    /// <summary>
    /// Build the graph starting with the given issues.
    /// </summary>
    /// <param name="jira">Already initialized instance to get ticket info from.</param>
    /// <param name="input">Options about what the graph should include.</param>
    /// <returns>A string containing the finished graph in Graphviz (gv/dot) format.</returns>
    public async Task<string> GetGraphvizData(JiraSearch jira, Options input)
    {
        var graphData = await GetGraphData(jira, input);
        var graphvizSource = graphData.Select(element => element.GetGraphvizCode(input.WordWrap)).ToList();

        var sb = new StringBuilder("digraph G {").AppendLine();

        sb.AppendLine($"\tnode [shape={input.NodeShape}];");
        sb.AppendLine("\trankdir=LR;");

        foreach (var line in graphvizSource)
        {
            sb.Append('\t');
            sb.AppendLine(line);
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    private async Task<List<IGraphElement>> GetGraphData(JiraSearch jira, Options input)
    {
        var graph = (Nodes: new HashSet<Node>(), Edges: new HashSet<Edge>());
        foreach (var inputIssue in input.Issues)
        {
            var (nodes, edges) = await BuildGraphData(inputIssue, jira, input.ExcludeLinks, input.ShowDirections,
                input.WalkDirections, input.Includes, input.IssueExcludes, input.IgnoreClosed, input.IncludeEpics,
                input.IncludeSubtasks, input.Traverse);
            graph.Nodes.UnionWith(nodes);
            graph.Edges.UnionWith(edges);
        }

        return graph.Nodes.Concat<IGraphElement>(graph.Edges).ToList();
    }

    /// <summary>
    /// Build the graph starting with the given issue.
    /// </summary>
    /// <param name="issue">Key of the issues to start the traversal with, e.g. "ABC-1234".</param>
    /// <param name="jira">Already initialized instance to get ticket info from.</param>
    /// <param name="excludeLinks">Links not to include in the graph e.g. "is blocked by".</param>
    /// <param name="showDirections">Directions to show in the graph. Default should be [inward, outward]</param>
    /// <param name="walkDirections"></param>
    /// <param name="includes"></param>
    /// <param name="issueExcludes"></param>
    /// <param name="ignoreClosed"></param>
    /// <param name="includeEpics"></param>
    /// <param name="includeSubtasks"></param>
    /// <param name="traverse"></param>
    /// <returns></returns>
    public async Task<(HashSet<Node> Nodes, HashSet<Edge> Edges)> BuildGraphData(string issue, JiraSearch jira, ISet<string> excludeLinks,
        ICollection<LinkDirection> showDirections, ISet<LinkDirection> walkDirections, string? includes, ISet<string> issueExcludes,
        bool ignoreClosed, bool includeEpics, bool includeSubtasks, bool traverse)
    {
        var nodes = new HashSet<Node>();
        var edges = new HashSet<Edge>();

        await TraverseIssue(issue, jira, excludeLinks, nodes, edges, showDirections, walkDirections, includes,
            issueExcludes, ignoreClosed, includeEpics, includeSubtasks, traverse, new HashSet<string>());

        return (nodes, edges);
    }

    private async Task TraverseIssue(string issue, JiraSearch jira, ISet<string> excludeLinks, ISet<Node> nodes,
        ISet<Edge> edges, ICollection<LinkDirection> showDirections, ISet<LinkDirection> walkDirections,
        string? includes,
        ISet<string> issueExcludes,
        bool ignoreClosed, bool includeEpics, bool includeSubtasks, bool traverse,
        ISet<string> seenIssues)
    {
        if (seenIssues.Contains(issue)) return;

        var issueInfo = await jira.GetIssue(issue);
        if (issueInfo == null) return;
        seenIssues.Add(issue);
        if (includes != null && !issueInfo.Key.Contains(includes)) return;

        if (issueExcludes.Any(exclude => issueInfo.Key.Contains(exclude)))
        {
            Console.WriteLine($"Skipping {issue} - explicitly excluded.");
            return;
        }

        nodes.Add(
            new Node(issueInfo)
        );

        // Process linked issues
        if (issueInfo.Fields.Links != null)
            foreach (var link in issueInfo.Fields.Links.Where(link => link.Target.Contains(includes ?? string.Empty)))
            {
                if (ignoreClosed && (link.OutwardIssue ?? link.InwardIssue)!.Fields.Status.IsClosed)
                {
                    Console.WriteLine($"Skipping {link.Target} - linked key is closed.");
                    continue;
                }

                if (showDirections.Contains(link.Direction) &&
                    !excludeLinks.Contains(link.DirectedLinkName.ToLower()))
                    edges.Add(new Edge(issueInfo, link));

                if (walkDirections.Contains(link.Direction) &&
                    (traverse || issue.Split('-')[0] == link.Target.Split('-')[0]))
                    await TraverseIssue(link.Target, jira, excludeLinks, nodes, edges, showDirections, walkDirections,
                        includes, issueExcludes, ignoreClosed, includeEpics, includeSubtasks, traverse,
                        seenIssues);
            }

        if (includeSubtasks && issueInfo.Fields.Subtasks != null)
            foreach (var subtask in issueInfo.Fields.Subtasks)
            {
                if (ignoreClosed && subtask.Fields.Status.IsClosed)
                {
                    Console.WriteLine($"Skipping {subtask.Key} - subtask is closed.");
                    continue;
                }

                if (showDirections.Contains(LinkDirection.Outward))
                    edges.Add(new Edge(issueInfo, subtask, "subtask"));

                if (walkDirections.Contains(LinkDirection.Outward))
                    await TraverseIssue(subtask.Key, jira, excludeLinks, nodes, edges, showDirections, walkDirections,
                        includes, issueExcludes, ignoreClosed, includeEpics, includeSubtasks, traverse,
                        seenIssues);
            }

        if(includeEpics && !string.IsNullOrWhiteSpace(issueInfo.Epic))
            await TraverseIssue(issueInfo.Epic, jira, excludeLinks, nodes, edges, showDirections, walkDirections,
                               includes, issueExcludes, ignoreClosed, includeEpics, includeSubtasks, traverse,
                                              seenIssues);
    }

    /// <summary>
    /// Build a graph from the given Jira issues. Check options doc for more info.
    /// </summary>
    /// <param name="jira">Ready-to-use (authenticated) jira connector.</param>
    /// <param name="input">Options for how to build the graph.</param>
    /// <returns>Dot code, SVG code, or a base64-encoded PNG.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if you pass an unknown OutputFormat</exception>
    public async Task<string> GetGraph(JiraSearch jira, Options input)
    {
        var graphvizSource = await GetGraphvizData(jira, input);

        var graph = input.Local
            ? input.OutputFormat switch
            {
                GraphFormat.Dot => graphvizSource,
                GraphFormat.Svg => await CompileGraphToSvgViaDot(graphvizSource),
                GraphFormat.Png => await CompileGraphToPngViaDot(graphvizSource),
                _ => throw new ArgumentOutOfRangeException(nameof(input.OutputFormat), input.OutputFormat, null)
            }
            : await CompileGraphToPngViaGoogle(input, graphvizSource);

        if (!string.IsNullOrWhiteSpace(input.ImageFile))
        {
            if (input.OutputFormat == GraphFormat.Png)
                await File.WriteAllBytesAsync(input.ImageFile, Convert.FromBase64String(graph));
            else await File.WriteAllTextAsync(input.ImageFile, graph);
        }

        return graph;
    }

    private async Task<string> CompileGraphToPngViaGoogle(Options input, string graphvizSource)
    {
        var chartUrl = $"{GoogleChartUrl}?cht=gv&chl={Uri.EscapeDataString(graphvizSource)}";
        chartUrl += $"&chls=transparent&chshape={input.NodeShape}";
        using var httpClient = new HttpClient();
        return Convert.ToBase64String(await httpClient.GetByteArrayAsync(chartUrl));
    }

    /// <summary>
    /// Uses the dot cli tool to compile the graphviz source to a png image.
    /// </summary>
    /// <param name="graphvizSource"></param>
    /// <returns>Base64 encoded PNG image source.</returns>
    /// <exception cref="Exception">Thrown if dot returns an error.</exception>
    private async Task<string> CompileGraphToPngViaDot(string graphvizSource)
    {
        var process = await SendToDot(graphvizSource, "-Tpng");

        string output;
        var errorTask = process.StandardError.ReadToEndAsync();
        using (var result = new MemoryStream())
        await using (var imageStream = process.StandardOutput.BaseStream)
        {
            await imageStream.CopyToAsync(result); // hangs here
            output = Convert.ToBase64String(result.ToArray());
        }

        var error = await errorTask;

        await process.WaitForExitAsync();

        if (process.ExitCode != 0) throw new Exception(error);

        return output;
    }

    private async Task<string> CompileGraphToSvgViaDot(string graphvizSource)
    {
        using var process = await SendToDot(graphvizSource, "-Tsvg");

        var errorTask = process.StandardError.ReadToEndAsync();
        var outputTask = process.StandardOutput.ReadToEndAsync();

        var error = await errorTask;
        var output = await outputTask;

        process.StandardError.Close();
        process.StandardOutput.Close();

        if (!process.HasExited)
        {
            await process.WaitForExitAsync();
        }

        if (process.ExitCode != 0) throw new Exception(error);

        return output;
    }


    private async Task<Process> SendToDot(string graphSource, string arguments)
    {
        var process1 = new Process
        {
            StartInfo =
            {
                FileName = "dot",
                Arguments = arguments,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process1.Start();
        var process = process1;

        await process.StandardInput.WriteAsync(graphSource);
        process.StandardInput.Close();

        return process;
    }
}