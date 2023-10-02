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

You can also use an OAuth token as authorization:

```bash
jiracli --cookie <Your OAuth token> SIERRA-117
```

The service will check the length to determine whether to use the session ID or the oauth token protocol.

### Web API

The frontend doesn't have all options yet. It's a simple html page with a form to enter options and jira issue IDs (multiple can be entered, separated by comma).  
By default this will simply walk the graph, ignore epics and subtasks, and build everything into an SVG, which will be diplayed on the page.  
The resulting SVG can be copied from dev tools. I might later add another button that puts the SVG into an `<img>` tag so it can simply be copied with a context click.

Kiosk mode can be started via the Kiosk mode button. This will hide the form and periodically reload the page with the same options. Default period is 1 minute. Refresh the page to immediately refresh the graph.

The backend supports all the options available to the CLI tool, there's a `/swagger` page to test it out.