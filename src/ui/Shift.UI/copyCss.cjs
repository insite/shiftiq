const fs = require("node:fs");
const path = require("node:path");

const uiFolder = path.resolve(process.cwd(), "public", "ui");
if (!fs.existsSync(uiFolder)) {
    fs.mkdirSync(uiFolder);
}

copyFile("/../../../source/InSite.UI/UI/Layout/Common/Styles/Shift.min.css");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Parts/around/js/vendor/simplebar/dist/simplebar.css");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Parts/around/js/vendor/simplebar/dist/simplebar.js");
copyFile("/../../../source/InSite.UI/UI/Layout/Common/Styles/Custom.min.css");

function copyFile(relativePath) {
    const src = path.resolve(process.cwd() + relativePath);
    const filename = path.basename(src);
    const dst = path.resolve(process.cwd(), "public", "ui", filename);

    fs.copyFileSync(src, dst);
}