# JiraTools

Library, CLI tool and WebAPI with minimal frontend to create a graph of Jira issues. Adapted from this python script: https://github.com/pawelrychlik/jira-dependency-graph  
Can include subtasks and epics.

Uses Jira REST API v2 to recursively load all given tickets and tickets linked to them.  
Then it creates a graph in [dot/gv format](https://graphviz.org/doc/info/lang.html) and renders it using a local dot cli tool or the Google Charts API.

## Usage

### CLI

This currently has more options than the Website. It's basically the original script with some other defaults.

Create a graph starting with Ticket SIERRA-117 and all tickets linked to it:

```bash
jiracli --cookie <Your JSESSIONID> SIERRA-117
```

Might create a graph with only one Node: SIERRA-117, if there are no subtasks, epics or linked tickets.  
If there are however, all those will be included in the graph.

There are options to customize that behaviour:

```bash
jiracli --cookie <Your JSESSIONID> --ignore-subtasks --ignore-epics SIERRA-117
```

This will ignore subtasks and epics and only include linked tickets.

### Web API

The frontend doesn't have many options yet. It's just an html page with a form to enter the basic needed data and jira issues IDs (multiple can be entered, separated by comma).  
For now this will simply walk the graph, ignore epics and subtasks, and build everything into an SVG, which will be diplayed on the page.  
The resulting SVG can be copied from dev tools.

The backend already supports all the options available to the CLI tool, there's a swagger page at `/swagger` to test it out.