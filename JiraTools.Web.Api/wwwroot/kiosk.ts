import { Options } from './options';
import * as GraphService from './graphService.js';

let storedOptions: Options | null = null;
const svgContainer = document.getElementById('svg-container');

function getOptionsFromStorage(): Options | null {
    const optionsString = sessionStorage.getItem('options');
    if (optionsString) {
        const options = JSON.parse(optionsString) as Options;
        options.OutputFormat = 'Svg';
        return options;
    }
    return null;
}

async function updateGraph() {
    // If we haven't loaded options before, try to get them from storage
    if (!storedOptions) {
        storedOptions = getOptionsFromStorage();
    }

    if (storedOptions) {
        try {
            const svgString = await createGraph(storedOptions);
            if (svgContainer) {
                svgContainer.innerHTML = svgString;
            }
        } catch (error) {
            if (svgContainer) {
                svgContainer.innerHTML = "<p>Error fetching graph. Please try again later.</p>";
            }
            console.error("Error fetching graph:", error);
        }
    } else {
        if (svgContainer) {
            svgContainer.innerHTML = "<p>Invalid or missing options. Please check the storage and try again.</p>";
        }
    }
}

async function createGraph(dto: Options): Promise<string> {
    if (GraphService.graphvizLoaded()) {
        dto.OutputFormat = "Dot";
        const dotCode = await GraphService.getGraph(dto);
        return await GraphService.dotSvg(dotCode);
    } else {
        dto.OutputFormat = "Svg";
        return await GraphService.getGraph(dto);
    }
}

// Initial call
updateGraph();

// Set an interval to update the graph every minute
setInterval(updateGraph, 60000);
