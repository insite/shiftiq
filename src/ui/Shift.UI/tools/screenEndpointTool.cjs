const fs = require("node:fs");
const path = require("node:path");

const srcFolder = path.resolve(__dirname, "..", "src");
const startFolder = path.resolve(srcFolder, "routes", "admin");

const screens = {};

const endpoints = {
    "shiftClient.achievement.retrieve": {
        url: "GET api/progress/achievements/{achievementId}",
        permission: "Policies.Progress.Achievements.Achievement.Retrieve"
    },
    "shiftClient.achievement.search": {
        url: "POST api/progress/achievements/search",
        permission: "Policies.Progress.Achievements.Achievement.Search",
    },
    "shiftClient.caseStatus.create": {
        url: "POST api/workflow/cases-statuses",
        permission: "Policies.Workflow.Cases.CaseStatus.Create"
    },
    "shiftClient.caseStatus.delete": {
        url: "DELETE api/workflow/cases-statuses/{statusId}",
        permission: "Policies.Workflow.Cases.CaseStatus.Delete"
    },
    "shiftClient.caseStatus.download": {
        url: "GET api/workflow/cases-statuses/download",
        permission: "Policies.Workflow.Cases.CaseStatus.Download"
    },
    "shiftClient.caseStatus.retrieve": {
        url: "GET api/workflow/cases-statuses/{statusId}",
        permission: "Policies.Workflow.Cases.CaseStatus.Retrieve"
    },
    "shiftClient.caseStatus.search": {
        url: "POST api/workflow/cases-statuses/search",
        permission: "Policies.Workflow.Cases.CaseStatus.Search"
    },
    "shiftClient.caseStatus.update": {
        url: "PUT api/workflow/cases-statuses/{statusId}",
        permission: "Policies.Workflow.Cases.CaseStatus.Modify"
    },
    "shiftClient.command.send": {
        url: "POST api/timeline/commands",
        permission: "Policies.Timeline.Commands.Send",
    },
    "shiftClient.event.retrieve": {
        url: "GET api/booking/events/{event}",
        permission: "Policies.Booking.Events.Event.Retrieve"
    },
    "shiftClient.file.download": {
        url: "POST api/content/files/download",
        permission: "Policies.Content.Files.File.Download"
    },
    "shiftClient.file.search": {
        url: "POST api/content/files/search",
        permission: "Policies.Content.Files.File.Search"
    },
    "shiftClient.gradebook.download": {
        url: "POST api/progress/gradebooks/download",
        permission: "Policies.Progress.Gradebooks.Gradebook.Download"
    },
    "shiftClient.gradebook.retrieve": {
        url: "GET api/progress/gradebooks/{gradebook}",
        permission: "Policies.Progress.Gradebooks.Gradebook.Retrieve"
    },
    "shiftClient.gradebook.search": {
        url: "POST api/progress/gradebooks/search",
        permission: "Policies.Progress.Gradebooks.Gradebook.Search"
    },
    "shiftClient.me.context": {
        url: "GET api/me/context",
        permission: "Public"
    },
    "shiftClient.organization.retrieve": {
        url: "GET api/security/organizations/{organization}",
        permission: "Policies.Security.Organizations.Organization.Retrieve"
    },
    "shiftClient.organization.search": {
        url: "POST api/security/organizations/search",
        permission: "Policies.Security.Organizations.Organization.Search"
    },
    "shiftClient.people.search": {
        url: "POST api/directory/people/search",
        permission: "Policies.Directory.People.Person.Search"
    },
    "shiftClient.period.retrieve": {
        url: "GET api/progress/periods/{period}",
        permission: "Policies.Progress.Periods.Period.Retrieve"
    },
    "shiftClient.period.search": {
        url: "POST api/progress/periods/search",
        permission: "Policies.Progress.Periods.Period.Search"
    },
    "shiftClient.standard.retrieve": {
        url: "POST api/competency/standards/{standard}",
        permission: "Policies.Competency.Standards.Standard.Retrieve"
    },
    "shiftClient.standard.search": {
        url: "POST api/competency/standards/search",
        permission: "Policies.Competency.Standards.Standard.Search"
    },
    "shiftClient.user.retrieve": {
        url: "GET api/security/users/{user}",
        permission: "Policies.Security.Users.User.Retrieve"
    },
    "shiftClient.user.search": {
        url: "POST api/security/users/search",
        permission: "Policies.Security.Users.User.Search"
    },
    "shiftClient.pageContent.modify": {
        url: "GET api/workspace/pages-contents/{page}",
        permission: "workspace/pages"
    },
    "shiftClient.pageContent.retrieve": {
        url: "PUT api/workspace/pages-contents/{page}",
        permission: "workspace/pages"
    },
    "shiftClient.file.uploadTempFile": {
        url: "POST api/content/files/temp",
        permission: "none"
    },
    "shiftClient.translation.translate": {
        url: "POST api/content/translations/translate",
        permission: "none"
    },
};

function processFolder(folderPath) {
    const files = [];
    const folders = [];

    readFolder(folderPath, files, folders);

    if (files.length > 0) {
        for (const subFolderPath of folders) {
            readFilesRecursively(subFolderPath, files);
        }
        console.log(getScreenUrl(folderPath));
        processScreenFiles(getScreenUrl(folderPath), files, []);
    } else {
        for (const subFolderPath of folders) {
            processFolder(subFolderPath);
        }
    }
}

function processScreenFiles(screenUrl, files, processedFiles) {
    const screen = screens[screenUrl] ?? (screens[screenUrl] = {});

    const importFiles = [];
    for(const file of files) {
        processedFiles.push(file);
    }

    for (const filePath of files) {
        const lines = fs.readFileSync(filePath, "utf-8").replaceAll("\r", "").split("\n");
        for (const line of lines) {
            if (addImportFile(filePath, line, importFiles, processedFiles)) {
                continue;
            }

            const startIndex = line.indexOf("shiftClient.");
            if (startIndex < 0) {
                continue;
            }
            const endIndex = line.indexOf("(", startIndex);
            const methodName = line.substring(startIndex, endIndex);

            const methodFiles = screen[methodName] ?? (screen[methodName] = []);

            if (!methodFiles.includes(path.basename(filePath))) {
                methodFiles.push(path.basename(filePath));
            }
        }
    }

    if (importFiles.length > 0) {
        processScreenFiles(screenUrl, importFiles, processedFiles);
    }
}

function addImportFile(filePath, line, importFiles, processedFiles) {
    if (!line.startsWith("import ")) {
        return false;
    }
    const fromIndex = line.indexOf(" from ");
    if (fromIndex < 0) {
        return true;
    }

    const startQuoteIndex = line.indexOf("\"", fromIndex);
    const endQuoteIndex = line.indexOf("\"", startQuoteIndex + 1);
    const fileName = line.substring(startQuoteIndex + 1, endQuoteIndex);

    let importFilePath;

    if (fileName.startsWith(".")) {
        importFilePath = path.resolve(path.dirname(filePath), fileName);
    } else if (fileName.startsWith("@/")) {
        importFilePath = path.resolve(srcFolder, fileName.substring(2));
    } else {
        return true;
    }

    if (fs.existsSync(importFilePath + ".tsx") && path.basename(importFilePath) !== "formRouteHelper") {
        importFilePath += ".tsx";
    } else if (fs.existsSync(importFilePath + ".ts") && path.basename(importFilePath).startsWith("use")) {
        importFilePath += ".ts";
    } else {
        importFilePath = null;
    }

    if (importFilePath
        && !importFiles.includes(importFilePath)
        && !processedFiles.includes(importFilePath)
    ) {
        importFiles.push(importFilePath);
    }

    return true;
}

function getScreenUrl(folderPath) {
    return "/client/admin" + folderPath.substring(startFolder.length).replaceAll("\\", "/");
}

function readFilesRecursively(folderPath, files) {
    const folders = [];

    readFolder(folderPath, files, folders);

    for(const folderPath of folders) {
        readFilesRecursively(folderPath, files);
    }
}

function readFolder(folderPath, files, folders) {
    const allFiles = fs.readdirSync(folderPath);

    for (const fileName of allFiles) {
        const filePath = path.resolve(folderPath, fileName);
        const stats = fs.statSync(filePath);

        if (stats.isDirectory()) {
            folders.push(filePath);
        } else if (stats.isFile() && (filePath.toLowerCase().endsWith(".ts") || filePath.toLowerCase().endsWith(".tsx"))) {
            files.push(filePath);
        }
    }
}

function setToArray(set) {
    const array = [];
    for (const key in set) {
        array.push(key);
    }
    array.sort();
    return array;
}

const mode = process.argv.length === 3
    ? (
        process.argv[2] === "-u"
            ? "url"
            : process.argv[2] === "-b"
                ? "urlAndPermission"
                : "permission"
    ) : "permission";

console.log("Screens:\n");
processFolder(startFolder);

console.log("\nEndpoints:");

const undefinedEndpoints = {};

for (const screenKey in screens) {
    console.log();
    console.log(screenKey);

    const methods = setToArray(screens[screenKey]);
    for (const method of methods) {
        const endpoint = endpoints[method];
        if (!endpoint?.permission) {
            undefinedEndpoints[method] = method;
        } else {
            switch (mode) {
                case "url":
                    console.log(`- ${endpoint.url}`);
                    break;
                case "urlAndPermission":
                    console.log(`- ${endpoint.url}: ${endpoint.permission}`);
                    break;
                default:
                    if (endpoint.permission !== "Public") {
                        console.log(`- ${endpoint.permission}`);
                    }
                    break;
            }
        }
    }
}

if (Object.keys(undefinedEndpoints).length > 0) {
    console.log("\nUndefined endpoints:");
    for (const method of setToArray(undefinedEndpoints)) {
        console.log(method);
    }
}