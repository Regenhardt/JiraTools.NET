namespace JiraTools.Web.Api.Controllers;

using JiraLib;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class JiraGraphController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <returns>B64-encoded PNG image of the created graph or the Graphviz code.</returns>
    [HttpPost]
    public async Task<string> BuildJiraGraph(Inputs options)
    {
        options.Clean();
        var jira = await (
            !string.IsNullOrWhiteSpace(options.User) && !string.IsNullOrWhiteSpace(options.Password)
                ? JiraSearch.CreateAsync(options.JiraUrl, options.User, options.Password)
                : JiraSearch.CreateAsync(options.JiraUrl, options.Cookie)
        );

        var graphService = new JiraGraphService();

        return options.ReturnSource
            ? await graphService.GetGraph(jira, options)
            : Convert.ToBase64String(await graphService.GetGraphAsPng(jira, options));
    }
}

public class Inputs : Options
{
    /// <summary>
    /// <inheritdoc cref="Options"/>
    /// </summary>
    public new List<string> ExcludeLinks { get; set; } = new();

    /// <summary>
    /// Just build and return the source Graphviz code, but doesn't convert it to PNG.
    /// </summary>
    public bool ReturnSource { get; set; }

    internal void Clean()
    {
        base.ExcludeLinks = new HashSet<string>(ExcludeLinks);
    }
}