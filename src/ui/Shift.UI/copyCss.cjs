const fs = require("node:fs");
const path = require("node:path");

const publicFolder = path.resolve(process.cwd(), "public");
if (!fs.existsSync(publicFolder)) {
    fs.mkdirSync(publicFolder);
}

const uiFolder = path.resolve(publicFolder, "ui");
if (!fs.existsSync(uiFolder)) {
    fs.mkdirSync(uiFolder);
}

fs.cpSync("local-assets", "public", { recursive: true });
fs.copyFileSync("../../../source/InSite.UI/favicon.ico", "public/favicon.ico");
fs.copyFileSync("../../../source/InSite.UI/favicon.png", "public/favicon.png");

copyFile("/../../../source/InSite.UI/UI/Layout/Common/Parts/around/js/vendor/simplebar/dist/simplebar.css");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Parts/around/js/vendor/simplebar/dist/simplebar.js");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Styles/shift.css");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Styles/skills.css");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Styles/cmds.css");

function copyFile(relativePath) {
    const src = path.resolve(process.cwd() + relativePath);
    const filename = path.basename(src);
    const dst = path.resolve(process.cwd(), "public", "ui", filename);

    fs.copyFileSync(src, dst);
}