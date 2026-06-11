import { shiftConfig } from "./shiftConfig";

const _orgStorageKey = "currentOrganization";
const _domainNameKey = "domainName";

function getActionUrl(relativeUrl: string) {
    return getUrl(relativeUrl);
}

function getUrl(relativeUrl: string) {
    if (!shiftConfig.isLocal || relativeUrl.toLowerCase().startsWith(shiftConfig.clientPrefix)) {
        return relativeUrl;
    }
    const origin = `https://local-${getOrg()}.${getDomainName()}`;
    return origin + relativeUrl;
}

function getOrg() {
    return localStorage.getItem(_orgStorageKey) || "insite";
}

function getDomainName() {
    return localStorage.getItem(_domainNameKey) || "insite.com";
}

function replaceUrlParams(url: string, params: string): string {
    const [query, queryParams] = url.split("?");
    if (!queryParams) {
        return `${query}?${params}`;
    }

    const queryParamsList = queryParams.split("&").map(x => x.split("="));
    const paramsList = params.split("&").map(x => x.split("="));
    let newQueryParams = "";

    for (const [name, value] of queryParamsList) {
        if (!name) {
            continue;
        }

        const newParam = paramsList.find(([newName]) => newName.toLowerCase() === name.toLowerCase());
        const newValue = newParam ? newParam[1] : value;

        newQueryParams += `&${name}=${newValue}`;
    }

    for (const [name, value] of paramsList) {
        if (queryParamsList.find(([oldName]) => oldName.toLowerCase() === name.toLowerCase())) {
            continue;
        }

        newQueryParams += `&${name}=${value}`;
    }

    return query + "?" + newQueryParams.substring(1);
}

export const urlHelper = {
    getLoginPageUrl() {
        const returnUrl = encodeURIComponent(window.location.pathname);
        return shiftConfig.isLocal
            ? `/client/react/signin?returnUrl=${returnUrl}`
            : getActionUrl(shiftConfig.loginPageUrl + `?returnUrl=${returnUrl}`);
    },

    get403PageUrl() {
        const path = encodeURIComponent(window.location.pathname);
        return getActionUrl(`/403?path=${path}`);
    },

    getActionUrl,

    getResourceUrl(relativeUrl: string) {
        return getUrl(relativeUrl);
    },

    getFileUrl(fileId: string, fileName: string) {
        return `${shiftConfig.shiftApiHostUrl}/api/assets/files/${fileId}/${fileName}`;
    },

    getFileUrlByNavigateUrl(navigateUrl: string) {
        return `${shiftConfig.shiftApiHostUrl}${navigateUrl}`;
    },

    getInSiteReturnUrl(params?: string): string {
        let returnUrl = shiftConfig.isLocal
            ? window.location.href
            : window.location.pathname;

        if (params) {
            returnUrl = replaceUrlParams(returnUrl, params);
        }

        return `return=.${encodeURIComponent(returnUrl)}`;
    },

    getOrg,

    setOrgAndDomain(organizationCode: string | undefined | null, domainName: string) {
        if (organizationCode) {
            localStorage.setItem(_orgStorageKey, organizationCode);
        } else {
            localStorage.removeItem(_orgStorageKey);
        }
        localStorage.setItem(_domainNameKey, domainName);
    },
}
