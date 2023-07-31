namespace JiraTools.Web.Api.Controllers;

using JiraLib;
using JiraLib.Services;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for building graphs from Jira issues.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class JiraGraphController : ControllerBase
{
    private static string? GraphvizWasmModule { get; set; }

    /// <summary>
    /// Build a graph from the given Jira issues. Check options doc for more info.
    /// </summary>
    /// <param name="options"><see cref="OptionsDto"/> object.</param>
    /// <returns>The created graph in the format set in <see cref="Options"/>.</returns>
    [HttpPost]
    public async Task<string> BuildJiraGraph(OptionsDto options)
    {
        options.Clean();
        var jira = await (
            !string.IsNullOrWhiteSpace(options.User) && !string.IsNullOrWhiteSpace(options.Password)
                ? JiraSearch.CreateAsync(options.JiraUrl, options.User, options.Password)
                : JiraSearch.CreateAsync(options.JiraUrl, options.Token)
        );

        var graphService = new JiraGraphService();

        // Build local until frontend can decide itself.
        options.Local = true;
        return await graphService.GetGraph(jira, options);
    }

    /// <summary>
    /// Load the hpcc graphviz wasm module.
    /// </summary>
    /// <returns>The module as a string.</returns>
    [HttpGet("wasm-module")]
    public async Task<IActionResult> GetGraphvizScript()
    {
        if (GraphvizWasmModule is null)
        {
            using var http = new HttpClient();
            GraphvizWasmModule = await http.GetStringAsync("https://cdn.jsdelivr.net/npm/@hpcc-js/wasm/dist/graphviz.umd.js");
        }

        return Content(GraphvizWasmModule, "application/javascript");
    }
}

/// <inheritdoc />
public class OptionsDto : Options
{
    /// <summary>
    /// <inheritdoc cref="Options"/>
    /// </summary>
    public new List<string> ExcludeLinks { get; set; } = new(); // Need list to receive as json.

    internal void Clean()
    {
        base.ExcludeLinks = new HashSet<string>(ExcludeLinks);

        // Don't create any files on the server
        ImageFile = string.Empty;
    }
}