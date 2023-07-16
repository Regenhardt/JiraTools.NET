using System.Text;
using JiraLib;
using JiraLib.Jira;

const string googleChartUrl = "https://chart.apis.google.com/chart";


string GetUserInput(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? throw new Exception("No input provided");
}

string GetPasswordInput(string prompt)
{
    Console.Write(prompt);
    var password = new StringBuilder();
    while (true)
    {
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }

        password.Append(key.KeyChar);
    }

    return password.ToString();
}

Options ParseArgs(string[] args)
{
    var options = new Options();
    var i = 0;
    while (i < args.Length)
    {
        switch (args[i])
        {
            case "-u":
            case "--user":
                options.User = args[++i];
                break;
            case "-p":
            case "--password":
                options.Password = args[++i];
                break;
            case "-c":
            case "--cookie":
            case "--token":
                options.Token = args[++i];
                break;
            case "-N":
            case "--no-auth":
                options.NoAuth = true;
                break;
            case "-j":
            case "--jira":
                options.JiraUrl = args[++i];
                break;
            case "-f":
            case "--file":
                options.ImageFile = args[++i];
                break;
            case "-l":
            case "--local":
                options.Local = true;
                break;
            case "-e":
            case "--ignore-epic":
                options.IncludeEpics = false;
                break;
            case "-x":
            case "--exclude-link":
                options.ExcludeLinks.Add(args[++i]);
                break;
            case "-ic":
            case "--ignore-closed":
                options.IgnoreClosed = true;
                break;
            case "-i":
            case "--issue-include":
                options.Includes = args[++i];
                break;
            case "-xi":
            case "--issue-exclude":
                options.IssueExcludes.Add(args[++i]);
                break;
            case "-sd":
            case "--show-directions":
                options.ShowDirections.Add(args[++i]);
                break;
            case "-d":
            case "--direction":
                options.WalkDirections.Add(args[++i]);
                break;
            case "-T":
            case "--dont-traverse":
                options.Traverse = false;
                break;
            case "-w":
            case "--word-wrap":
                options.WordWrap = true;
                break;
            case "-q":
            case "--jql":
                options.JqlQuery = args[++i];
                break;
            case "--ignore-subtasks":
                options.IncludeSubtasks = false;
                break;
            case "--node-shape":
                options.NodeShape = args[++i];
                break;
            case "--output_format":
                options.OutputFormat = Enum.Parse<GraphFormat>(args[++i], true);
                break;
            default:
                options.Issues.Add(args[i]);
                break;
        }

        i++;
    }

    return options;
}

async Task CreateGraphImage(string graph, string imageFile, string nodeShape)
{
    var chartUrl = $"{googleChartUrl}?cht=gv&chl={Uri.EscapeDataString(graph)}";
    chartUrl += $"&chls=transparent&chshape={nodeShape}";

    using var httpClient = new HttpClient();
    var imageData = await httpClient.GetByteArrayAsync(chartUrl);
    await File.WriteAllBytesAsync(imageFile, imageData);

    Console.WriteLine($"Graph image saved to: {imageFile}");
}

var options = ParseArgs(args);

string? auth;
if (options.Token != null)
{
    auth = options.Token;
}
else if (options.NoAuth)
{
    auth = null;
}
else
{
    var user = options.User ?? GetUserInput("Username: ");
    var password = options.Password ?? GetPasswordInput("Password: ");
    auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
}

var jira = await JiraSearch.CreateAsync(options.JiraUrl, auth);

if (options.JqlQuery != null) options.Issues.AddRange(await jira.ListIds(options.JqlQuery));

var graph = await new JiraGraphService().GetGraphvizData(jira, options);

if (options.Local)
    Console.WriteLine(graph);
else
    await CreateGraphImage(graph, options.ImageFile, options.NodeShape);
