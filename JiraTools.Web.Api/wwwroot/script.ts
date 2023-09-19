﻿import { Options } from './options';

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

    get Token(): string {
        return this.str("token");
    }

    get AuthType(): 'username' | 'jsessionid' {
        return this.radio("authType") as 'username' | 'jsessionid';
    }

    get Issues(): string {
        return this.str("issues");
    }

    get IncludeSubtasks(): boolean {
        return this.b("includeSubtasks");
    }

    get IncludeEpics(): boolean {
        return this.b("includeEpics");
    }

    get IgnoreClosed(): boolean {
        return this.b("ignoreClosed");
    }

    get ExcludeLinks(): string {
        return this.str("excludeLinks") || "";
    }
}

async function getGraph(dto: Options): Promise<string> {
    if (graphElement == null) {
        alert("Can't find div to put the graph into, dev fucked up.");
        return '';
    }

    graphElement.innerHTML = '';

    const graphResponse = await fetch("/api/jiragraph", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(dto)
    });

    if (graphResponse.ok) {
        return await graphResponse.text();
    } else {
        const error: any = (await graphResponse.json());
        console.log("Error", error);
        throw `${error.detail.replace("401 ()", "401 Invalid Jira Authorization")}`;
    }
}

/**
 * Build a DTO fro the backend from the data of the form.
 * @param form The form to query the data from.
 * @returns A finished DTO like the backend expects it.
 */
function getDto(form: JiraGraphForm): Options {
    var options: Options = {
        NoAuth: !(form.Password || form.Token),
        JiraUrl: form.JiraUrl,
        User: form.Username,
        Password: form.Password,
        Token: form.Token,
        IncludeEpics: form.IncludeEpics,
        ExcludeLinks: form.ExcludeLinks?.split(',').map((i => i.trim())).filter(i => i),
        IgnoreClosed: form.IgnoreClosed,
        IssueExcludes: [],
        ShowDirections: undefined,
        WalkDirections: undefined,
        Traverse: true,
        WordWrap: true,
        Issues: form.Issues.split(',').map((i => i.trim())),
        IncludeSubtasks: form.IncludeSubtasks,
        NodeShape: 'box',
    };

    return options;
}

function imgTag(src: string) {
    return `<img src="${src}" alt="Image of a nice (or maybe not so nice) graph." />`;
}

/**
 * Sets the png data into an img tag and that to the inner html of the graph element.
 * @param pngData Base64 encoded png data.
 */
function setPng(pngData: string): void {
    graphElement.innerHTML = imgTag(`data:image/png;base64,${pngData}`);
}

/**
 * Sets the svg data to the inner html of the graph element.
 * @param svgData The svg data to set.
 */
function setSvg(svgData: string): void {
    graphElement.innerHTML = svgData;
}

let graphviz: any | null = null;

let lockForm = false;
export async function createGraph(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    if (lockForm) {
        return;
    }

    const submitButton = event.submitter as HTMLButtonElement;
    submitButton.disabled = lockForm = true;

    const form = new JiraGraphForm(event.target as HTMLFormElement);
    const dto = getDto(form);
    switch (event.submitter!.id) {
        case "PNG":
            dto.OutputFormat = "Png";
            break;
        case "SVG":
            dto.OutputFormat = "Svg";
            break;
        default:
            dto.OutputFormat = "Dot";
            break;
    }

    try {
        if (!graphviz) {
            await createGraphOnServer(dto);
        } else {
            await createGraphInBrowser(dto);
        }
    } catch (error: any) {
        graphElement.innerHTML = `<pre>${error}</pre>`;
    }

    submitButton.disabled = lockForm = false;
}

async function createGraphOnServer(dto: Options) {
    let graphData = await getGraph(dto);
    if (graphData.length > 0) {
        switch (dto.OutputFormat) {
            case 'Png':
                setPng(graphData);
                break;
            case 'Svg':
                setSvg(graphData);
                break;
            default:
                graphElement.innerHTML = `<pre>${graphData}</pre>`;
                break;
        }
    }
}

async function createGraphInBrowser(dto: Options) {
    if (!graphviz) {
        alert("Can't compile dot in browser, graphviz module not loaded.");
        return;
    }

    const outputFormat = dto.OutputFormat;
    dto.OutputFormat = "Dot";
    const dotCode = await getGraph(dto);
    dto.OutputFormat = outputFormat;
    switch (outputFormat) {
        case 'Png':
            const pngData = await graphviz.dot(dotCode, "png");
            setPng(pngData);
            break;
        case 'Svg':
            const svgData = await graphviz.dot(dotCode, "svg");
            setSvg(svgData);
            break;
        default:
            graphElement.innerHTML = `<pre>${dotCode}</pre>`;
            break;
    }
}

(window as any).createGraph = createGraph;


async function getGraphvizWasm(): Promise<any> {
    const response = await fetch("api/jiragraph/wasm-module");
    const script = await response.text();
    eval(script);
    const wasmModule: any = (window as { [key: string]: any })["@hpcc-js/wasm"] as any;
    return await wasmModule.Graphviz.load();
}

export async function loadGraphvizWasm() {
    graphviz = await getGraphvizWasm();
    console.log("Graphviz wasm module loaded", graphviz.version());
}

(window as any).loadGraphvizWasm = loadGraphvizWasm;
loadGraphvizWasm();

/**
 * Here be style stuff.
 */
window.addEventListener('load', () => {
    adjustContentMargin();
});

window.addEventListener('resize', () => {
    adjustContentMargin();
});

function adjustContentMargin() {
    const header = document.getElementById('header');
    const content = document.getElementById('graph');
    const headerHeight = header?.offsetHeight;

    if(content)
        content.style.marginTop = `${headerHeight}px`;
}
