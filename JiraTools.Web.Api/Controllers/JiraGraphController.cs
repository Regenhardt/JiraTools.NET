namespace JiraTools.Web.Api.Controllers;

using JiraLib;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class JiraGraphController : ControllerBase
{
    [HttpPost]
    public async Task<string> BuildJiraGraph(Inputs options)
    {
        options.Clean();
        var jira = await (
            options.User != null && options.Password != null
                ? JiraSearch.CreateAsync(options.JiraUrl, options.User, options.Password)
                : JiraSearch.CreateAsync(options.JiraUrl, options.Cookie)
        );

        var graphService = new JiraGraphService();
        var graphvizData = await graphService.GetGraph(jira, options);

        return graphvizData;
    }
}

public class Inputs : Options
{
    /// <summary>
    /// <inheritdoc cref="Options"/>
    /// </summary>
    public new List<string> ExcludeLinks { get; set; } = new();

    internal void Clean()
    {
        base.ExcludeLinks = new HashSet<string>(ExcludeLinks);
    }
}