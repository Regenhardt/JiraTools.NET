import { Options } from './options';

const graphElement: HTMLImageElement = document.getElementById("graph") as HTMLImageElement;
console.log("Script loaded");

class JiraGraphForm {
    private elements: HTMLFormControlsCollection;

    constructor(form: HTMLFormElement) {
        this.elements = form.elements;
    }

    /**
     * Get a string from an input field
     * @param name Name of the input field in the form.
     * @returns A string.
     */
    private str(name: string): string {
        return (this.elements.namedItem(name) as HTMLInputElement).value;
    }

    /**
     * Get a boolean from a checkbox
     * @param name Name of the checkbox in the form.
     * @returns A boolean.
     */
    private b(name: string): boolean {
        return (this.elements.namedItem(name) as HTMLInputElement).checked;
    }

    private radio(name: string): string {
        return (this.elements.namedItem(name) as RadioNodeList).value;
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
        return this.radio("authType") as 'username' | 'jsessionid';
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

    const graphResponse = await fetch("/api/jiragraph", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(options)
    });

    if (graphResponse.ok) {
        const base64PngData = await graphResponse.text();
        graphElement.src = "data:image/png;base64," + base64PngData;
    } else {
        const error: string = (await graphResponse.json()).detail.replace("401 ()", "401 Invalid Jira Authorization");
        // Put into <pre> into graphElement
        graphElement.innerHTML = `<pre>${error}</pre>`;
    }
}

(window as any).createGraph = createGraph;