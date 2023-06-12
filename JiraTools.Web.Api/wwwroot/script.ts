import { Options } from './options';

const graphElement = document.getElementById("graph");
console.log("Script loaded");

class JiraGraphForm {
    private elements: HTMLFormControlsCollection;

    constructor(form: HTMLFormElement) {
        this.elements = form.elements;
    }

    private str(name: string): string {
        return (this.elements.namedItem(name) as Element).textContent as string;
    }

    private b(name: string): boolean {
        return (this.elements.namedItem(name) as HTMLInputElement).checked;
    }

    get JiraUrl(): string {
        return this.str("JiraUrl");
    }

    get Username(): string {
        return this.str("Username");
    }

    get Password(): string {
        return this.str("Password");
    }

    get Cookie(): string {
        return this.str("Cookie");
    }

    get AuthType(): 'username' | 'jsessionid' {
        return (this.elements.namedItem("authType") as Element).nodeValue as 'username' | 'jsessionid';
    }

    get IncludeSubtasks(): boolean {
        return this.b("IncludeSubtasks");
    }
}

export async function createGraph(event: Event) {
    event.preventDefault();
    const form = new JiraGraphForm(event.target as HTMLFormElement);

    var options: Options = {
        NoAuth: undefined,
        JiraUrl: form.JiraUrl,
        ImageFile: undefined,
        Local: undefined,
        IncludeEpics: false,
        ExcludeLinks: new Set(),
        IgnoreClosed: false,
        IssueExcludes: new Set(),
        ShowDirections: undefined,
        WalkDirections: undefined,
        Traverse: true,
        WordWrap: false,
        Issues: [],
        IncludeSubtasks: false,
        NodeShape: 'box'
    };

    const graphData = await fetch("/api/jiragraph", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(options)
    });

    const graphSvg = await graphData.text();
    if (graphElement == null) {
        alert("Can't find div to put the graph into, dev fucked up.");
        return;
    }
    graphElement.innerHTML = graphSvg;
}
