import { INSITE_LOCAL_DOMAIN, LOGIN_PAGE_URL, SHIFT_API_HOST_URL, IS_LOCAL } from "./constants";

const _orgStorageKey = "currentOrganization";

function getActionUrl(relativeUrl: string) {
    return getUrl(relativeUrl);
}

function getUrl(relativeUrl: string) {
    if (!IS_LOCAL) {
        return relativeUrl;
    }
    const origin = `https://local-${getOrg()}.${INSITE_LOCAL_DOMAIN}`;
    return origin + relativeUrl;
}

function getOrg() {
    return localStorage.getItem(_orgStorageKey) || "insite";
}

export const urlHelper = {
    getLoginPageUrl() {
        const returnUrl = encodeURIComponent(window.location.pathname);
        return IS_LOCAL
            ? `/client/react/signin?returnUrl=${returnUrl}`
            : getActionUrl(LOGIN_PAGE_URL + `?returnUrl=${returnUrl}`);
    },

    getActionUrl,

    getResourceUrl(relativeUrl: string) {
        return getUrl(relativeUrl);
    },

    getFileUrl(fileId: string, fileName: string) {
        return `${SHIFT_API_HOST_URL}/content/files/${fileId}/${encodeURIComponent(fileName)}`;
    },

    getOrg,

    setOrg(organizationCode: string) {
        if (organizationCode) {
            localStorage.setItem(_orgStorageKey, organizationCode);
        } else {
            localStorage.removeItem(_orgStorageKey);
        }
    },
}