import { Options } from './options';

const graphElement = document.getElementById("graph");
console.log("Script loaded");

class JiraGraphForm {
    private elements: HTMLFormControlsCollection;

    constructor(form: HTMLFormElement) {
        this.elements = form.elements;
    }

    private str(name: string): string {
        return (this.elements.namedItem(name) as HTMLInputElement).value;
    }

    private b(name: string): boolean {
        return (this.elements.namedItem(name) as HTMLInputElement).checked;
    }

    get JiraUrl(): string {
        return this.str("jiraUrl");
    }

    get Username(): string {
        return this.str("username");
    }

    get Password(): string {
        return this.str("password");
    }

    get Cookie(): string {
        return this.str("cookie");
    }

    get AuthType(): 'username' | 'jsessionid' {
        return (this.elements.namedItem("authType") as Element).nodeValue as 'username' | 'jsessionid';
    }

    get IncludeSubtasks(): boolean {
        return this.b("includeSubtasks");
    }

    get Issues(): string {
        return this.str("issues");
    }
}

export async function createGraph(event: Event) {
    console.log("createGraph called");
    if (graphElement == null) {
        alert("Can't find div to put the graph into, dev fucked up.");
        return;
    }
    event.preventDefault();
    const form = new JiraGraphForm(event.target as HTMLFormElement);

    var options: Options = {
        NoAuth: undefined,
        JiraUrl: form.JiraUrl,
        User: form.Username,
        Password: form.Password,
        Cookie: form.Cookie,
        ImageFile: undefined,
        Local: undefined,
        IncludeEpics: false,
        ExcludeLinks: [],
        IgnoreClosed: false,
        IssueExcludes: [],
        ShowDirections: undefined,
        WalkDirections: undefined,
        Traverse: true,
        WordWrap: false,
        Issues: form.Issues.split(',').map((i => i.trim())),
        IncludeSubtasks: false,
        NodeShape: 'box',
    };

    const graphData = await fetch("/api/jiragraph", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(options)
    });

    if (graphData.ok) {
        const graphSvg = await graphData.text();
        graphElement.innerHTML = graphSvg;
    } else {
        const error: string = (await graphData.json()).detail.replace("401 ()", "401 Invalid Jira Authorization");
        // Put into <pre> into graphElement
        graphElement.innerHTML = `<pre>${error}</pre>`;
    }
}

(window as any).createGraph = createGraph;