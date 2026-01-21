import { shiftConfig } from "./shiftConfig";

export const cssHelper = {
    setMainCssFiles() {
        (document.getElementById("shift_stylesheet") as HTMLLinkElement).href = shiftConfig.shiftCssUrl;
        (document.getElementById("font_awesome_stylesheet") as HTMLLinkElement).href = shiftConfig.fontAwesomeUrl;
        (document.getElementById("simplebar_stylesheet") as HTMLLinkElement).href = shiftConfig.simplebarCssUrl;
        (document.getElementById("simplebar_script") as HTMLScriptElement).src = shiftConfig.simplebarJsUrl;
    },
    setShiftCssFile(cssUrl: string)  {
        if (shiftConfig.isLocal) {
            const index = cssUrl.lastIndexOf("/");
            cssUrl = `/ui${cssUrl.substring(index)}`;
        }
        (document.getElementById("shift_stylesheet") as HTMLLinkElement).href = cssUrl;
    }
}