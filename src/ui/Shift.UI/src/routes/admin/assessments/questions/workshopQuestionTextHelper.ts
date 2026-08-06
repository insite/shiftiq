import { Language, MultiLanguageText } from "@/helpers/language";

function toLocalMarkdown(serverText: string) {
    if (!serverText) {
        return "";
    }
    return serverText
        .replaceAll(/(&(o|c)cb;|{|})/g, function (m) {
            if (m.length > 1) {
                return "&" + m;
            }
            if (m === "{") {
                return "&ocb;";
            }
            if (m === "}") {
                return "&ccb;";
            }
            return m;
        })
        .replaceAll(/[^\s]+(&nbsp;[^\s]*)+/g, function (m) {
            return "{" + m.replaceAll(/&nbsp;/g, " ") + "}"
        })
        .replaceAll(/&?&(o|c)cb;/g, function (m) {
            if (m.length > 5) {
                return m.substring(1);
            }
            if (m === "&ocb;") {
                return "{{";
            }
            if (m === "&ccb;") {
                return "}}";
            }
            return m;
        })
        ;
}

function toServerMarkdown(localText: string) {
    if (!localText) {
        return "";
    }
    return localText
        .replaceAll(/(&(o|c)cb;|{{|}})/g, function (m) {
            if (m.length > 2) {
                return "&" + m;
            }
            if (m ==="{{") {
                return "&ocb;";
            }
            if (m === "}}") {
                return "&ccb;";
            }
            return m;
        })
        .replaceAll(/{[^{}]*}/g, function (m) {
            return m.substring(1, m.length - 1).replaceAll(/\s/g, "&nbsp;")
        })
        .replaceAll(/&?&(o|c)cb;/g, function (m) {
            if (m.length > 5) {
                return m.substring(1);
            }
            if (m === "&ocb;") {
                return "{";
            }
            if (m === "&ccb;") {
                return "}";
            }
            return m;
        })
        ;
}

export const workshopQuestionTextHelper = {
    toLocalMarkdown,
    toServerMarkdown,

    toLocalMultiLanguage(serverValue: MultiLanguageText): MultiLanguageText {
        const result: MultiLanguageText = {};
        for (const key in serverValue) {
            if (serverValue[key as Language]) {
                result[key as Language] = toLocalMarkdown(serverValue[key as Language]!);
            }
        }
        return result;
    },    

    toServerMultiLanguage(localValue: MultiLanguageText): MultiLanguageText {
        const result: MultiLanguageText = {};
        for (const key in localValue) {
            if (localValue[key as Language]) {
                result[key as Language] = toServerMarkdown(localValue[key as Language]!);
            }
        }
        return result;
    },
};