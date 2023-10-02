import { Options } from "./options";

let graphviz: any | null = null;
async function getGraphvizWasm(): Promise<any> {
    const response = await fetch("api/jiragraph/wasm-module");
    const script = await response.text();
    eval(script);
    const wasmModule: any = (window as { [key: string]: any })["@hpcc-js/wasm"] as any;
    return await wasmModule.Graphviz.load();
}

export async function loadGraphvizWasm(): Promise<void> {
    graphviz = await getGraphvizWasm();
    console.log("Graphviz wasm module loaded", graphviz.version());
}

export function graphvizLoaded():boolean {
    return graphviz != null;
}

export async function dotSvg(dotCode: string): Promise<string> {
    return await graphviz.dot(dotCode, "svg");
}

export async function dotPng(dotCode: string) {
    return await graphviz.dot(dotCode, "png");
}

export async function getGraph(dto: Options) {
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

(window as any).loadGraphvizWasm = loadGraphvizWasm;
loadGraphvizWasm();