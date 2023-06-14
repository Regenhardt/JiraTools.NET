# JiraTools

Library, CLI tool and WebAPI with minimal frontend to create a graph of Jira issues. Adapted from this python script: https://github.com/pawelrychlik/jira-dependency-graph  
Can include subtasks and epics.

Uses Jira REST API v2 to recursively load all given tickets and tickets linked to them.  
Then it creates a graph in [dot/gv format](https://graphviz.org/doc/info/lang.html) and renders it to PNG using the Google Charts API.

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