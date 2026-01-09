import { shiftConfig } from "./shiftConfig";

const _orgStorageKey = "currentOrganization";

function getActionUrl(relativeUrl: string) {
    return getUrl(relativeUrl);
}

function getUrl(relativeUrl: string) {
    if (!shiftConfig.isLocal) {
        return relativeUrl;
    }
    const origin = `https://local-${getOrg()}.${shiftConfig.insiteLocalDomain}`;
    return origin + relativeUrl;
}

function getOrg() {
    return localStorage.getItem(_orgStorageKey) || "insite";
}

export const urlHelper = {
    getLoginPageUrl() {
        const returnUrl = encodeURIComponent(window.location.pathname);
        return shiftConfig.isLocal
            ? `/client/react/signin?returnUrl=${returnUrl}`
            : getActionUrl(shiftConfig.loginPageUrl + `?returnUrl=${returnUrl}`);
    },

    getActionUrl,

    getResourceUrl(relativeUrl: string) {
        return getUrl(relativeUrl);
    },

    getFileUrl(fileId: string, fileName: string) {
        return `${shiftConfig.shiftApiHostUrl}/content/files/${fileId}/${encodeURIComponent(fileName)}`;
    },

    getOrg,

    setOrg(organizationCode: string | undefined | null) {
        if (organizationCode) {
            localStorage.setItem(_orgStorageKey, organizationCode);
        } else {
            localStorage.removeItem(_orgStorageKey);
        }
    },
}