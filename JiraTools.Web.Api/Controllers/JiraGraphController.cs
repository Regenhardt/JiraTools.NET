namespace JiraTools.Web.Api.Controllers;

using JiraLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

[Route("api/[controller]")]
[ApiController]
public class JiraGraphController : ControllerBase
{
    [HttpPost]
    public async Task<string> BuildJiraGraph(Options options)
    {
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
