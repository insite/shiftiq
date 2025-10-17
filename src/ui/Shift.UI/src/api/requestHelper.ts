import { SHIFT_API_HOST_URL } from "@/helpers/constants";
import { urlHelper } from "@/helpers/urlHelper";
import { timerHelper } from "@/helpers/timerHelper";
import { ApiError } from "./apiError";

export interface Param {
    name: string;
    value: string | number | undefined | null;
}

export const requestHelper = {
    beforeRequest(relativeUrl: string, params: Param[] | undefined | null) {
        return requestHelper.appendParams(SHIFT_API_HOST_URL + relativeUrl, params);
    },

    afterRequest(isOk: boolean, status: number, json: unknown): unknown {
        if (isOk) {
            timerHelper.resetTimer();
            return json;
        }

        if (status === 403 || status === 401) {
            window.location.href = urlHelper.getLoginPageUrl();
            return null;
        }

        let error: string;

        if (typeof json === "string") {
            error = json
        } else {
            const errorObj = json as {
                Summary: string | undefined,
                Message: string | undefined,
                error: string | undefined,
                title: string | undefined,
                Detail: string | undefined,
            };
            error = errorObj.Summary ?? errorObj.Message ?? errorObj.error ?? errorObj.title ?? errorObj.Detail ?? "Unknown Error";
        }

        throw new ApiError(status, error);
    },

    appendParams(url: string, params: Param[] | undefined | null): string {
        if (!params) {
            return url;
        }

        let first = true;
        for (const { name, value } of params) {
            if (!value) {
                continue;
            }
            url += first ? "?" : "&";
            url += `${name}=${encodeURIComponent(value)}`;
            first = false;
        }

        return url;
    },
};
