import { shiftConfig } from "./shiftConfig";

export const cssHelper = {
    setMainCssFiles() {
        (document.getElementById("shift_stylesheet") as HTMLLinkElement).href = shiftConfig.shiftCssUrl;
        (document.getElementById("font_awesome_stylesheet") as HTMLLinkElement).href = shiftConfig.fontAwesomeUrl;
        (document.getElementById("simplebar_stylesheet") as HTMLLinkElement).href = shiftConfig.simplebarCssUrl;
        (document.getElementById("simplebar_script") as HTMLScriptElement).src = shiftConfig.simplebarJsUrl;
    },
    setCustomCssFile(customCssUrl: string)  {
        if (shiftConfig.isLocal) {
            switch (customCssUrl.toLowerCase()) {
                case "/ui/layout/common/styles/cmds.css":
                    customCssUrl = "/ui/Cmds.css";
                    break;
                case "/ui/layout/common/styles/skills.css":
                    customCssUrl = "/ui/Skills.css";
                    break;
                default:
                    throw new Error(`Unsupported custom css url: ${customCssUrl}`);
            }
        }
        (document.getElementById("custom_stylesheet") as HTMLLinkElement).href = customCssUrl;
    }
}