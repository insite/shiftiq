const fs = require("node:fs");
const path = require("node:path");

if (process.argv.length < 4) {
    throw new Error("Expected partition and environment argurments");
}

const partition = process.argv[2].toLowerCase();
const environment = process.argv[3].toLowerCase();

let prefix;
switch (environment) {
    case "production":
        prefix = "";
        break;
    case "development":
        prefix = "dev-";
        break;
    case "sandbox":
        prefix = "sandbox-";
        break;
    case "local":
        prefix = "";
        break;
    default:
        throw new Error(`Environment is not supported: ${environment}`);
}

let domain;
switch (partition) {
    case "e01":
    case "e05":
    case "e07":
    case "e08":
    case "e99":
        domain = "shiftiq.com";
        break;
    case "e02":
    case "e04":
    case "e06":
        domain = "insite.com";
        break;
    case "e03":
        domain = "cmds.app";
        break;
    default:
        throw new Error(`Partition is not supported: ${environment}`);
}

const apiUrl = environment === "local"
    ? "https://localhost:5000"
    : `https://${prefix}api.${domain}/v2/${partition}`;

console.log(`API Url: ${apiUrl}`);

const startFolder = __dirname;

processFolder(path.resolve(startFolder, "assets"));

function processFolder(folderPath) {
    const files = fs.readdirSync(folderPath);
    for (const fileName of files) {
        const filePath = path.resolve(folderPath, fileName);
        const stats = fs.statSync(filePath);

        if (stats.isDirectory()) {
            processFolder(filePath);
        } else if (stats.isFile() && filePath.toLowerCase().endsWith(".js")) {
            processFile(filePath);
        }
    }
}

function processFile(filePath) {
    console.log(`Processing file ${filePath} ...`);

    const fileContent = fs.readFileSync(filePath, "utf-8");

    const newFileContent = fileContent.replace("_api_insite_com_", apiUrl);

    if (fileContent !== newFileContent) {
        fs.writeFileSync(filePath, newFileContent);
        console.log("Replaced.");
    }
}