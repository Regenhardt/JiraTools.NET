import { Options } from "./options";

let graphviz: any | null = null;
let graphvizLoading: Promise<any> | null = null;

async function getGraphvizWasm(): Promise<any> {
    try {
        const response = await fetch("api/jiragraph/wasm-module");
        if (!response.ok) {
            throw new Error(`Failed to fetch wasm module: ${response.status} ${response.statusText}`);
        }
        
        const script = await response.text();
        
        // Execute the script to load the UMD module
        eval(script);
        
        let wasmModule: any = (window as any)["@hpcc-js/wasm/graphviz"] || (window as any)["@hpcc-js/wasm"] || (window as any).hpccWasm;
        
        if (!wasmModule) {
            console.error("Available window properties:", Object.keys(window));
            throw new Error("Could not find @hpcc-js/wasm module in window after loading script");
        }
        
        if (!wasmModule.Graphviz) {
            throw new Error("Graphviz not found in wasm module");
        }
        
        console.log("Loading Graphviz instance...");
        return await wasmModule.Graphviz.load();
    } catch (error) {
        console.error("Error loading Graphviz wasm module:", error);
        throw error;
    }
}

export async function loadGraphvizWasm(): Promise<void> {
    if (!graphvizLoading) {
        graphvizLoading = getGraphvizWasm();
    }
    graphviz = await graphvizLoading;
}

(window as any).loadGraphvizWasm = loadGraphvizWasm;
loadGraphvizWasm();

export async function graphvizLoaded(): Promise<boolean> {
    if (graphvizLoading) {
        try {
            await graphvizLoading;
        } catch (error) {
            console.error("Graphviz failed to load:", error);
            return false;
        }
    }
    return !!graphviz;
}

export async function dotSvg(dotCode: string): Promise<string> {
    if (!graphviz) {
        await loadGraphvizWasm();
    }
    if (!graphviz) {
        throw new Error("Graphviz wasm module not loaded");
    }
    return await graphviz.dot(dotCode, "svg");
}

export async function dotPng(dotCode: string) {
    if (!graphviz) {
        await loadGraphvizWasm();
    }
    if (!graphviz) {
        throw new Error("Graphviz wasm module not loaded");
    }
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
