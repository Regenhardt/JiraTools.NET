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
    /// Get metadata about this controller.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetControllerMetadata()
    {
        var controllerDescription = "Controller for building graphs from Jira issues.";

        var endpoints = new List<object>
        {
            new
            {
                Method = "POST",
                Route = "api/JiraGraph",
                Description = "Build a graph from the given Jira issues based on the provided options."
            },
            new
            {
                Method = "GET",
                Route = "api/JiraGraph/wasm-module",
                Description = "Load the hpcc graphviz wasm module."
            }
        };

        var result = new
        {
            Controller = nameof(JiraGraphController),
            Description = controllerDescription,
            Endpoints = endpoints
        };

        return Ok(result);
    }

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
            !string.IsNullOrWhiteSpace(options.Password) && string.IsNullOrWhiteSpace(options.Token)
                ? JiraSearch.CreateFromPasswordAsync(options.JiraUrl, options.User, options.Password)
                : JiraSearch.CreateFromTokenAsync(options.JiraUrl, options.User, options.Token)
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
    [ResponseCache(Duration = 3600)] // Cache for 1 hour since the module doesn't change often
    public async Task<IActionResult> GetGraphvizScript()
    {
        try
        {
            if (GraphvizWasmModule is null)
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(30);
                GraphvizWasmModule = await http.GetStringAsync("https://cdn.jsdelivr.net/npm/@hpcc-js/wasm/dist/graphviz.umd.js");
            }

            return Content(GraphvizWasmModule, "application/javascript; charset=utf-8");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to load Graphviz wasm module", details = ex.Message });
        }
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
        base.ExcludeLinks = new HashSet<string>(ExcludeLinks.Select(linkName => linkName.ToLower()));

        // Don't create any files on the server
        ImageFile = string.Empty;
    }
}