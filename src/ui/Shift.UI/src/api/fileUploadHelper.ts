import { ApiError } from "./apiError";
import { Param, requestHelper } from "./requestHelper";

function createFormData(files: FileList | File): FormData {
    const formData = new FormData();

    if (files instanceof FileList) {
        for (const file of files) {
            formData.append(file.name, file);
        }
    } else {
        formData.append(files.name, files);
    }

    return formData;
}

function onLoad<T>(
    xhr: XMLHttpRequest,
    resolve: (value: T | null) => void,
    reject: (reason: unknown) => void
) {
    const contentType = xhr.getResponseHeader("Content-Type");
    let json: unknown;

    try {
        json = contentType?.includes("application/json")
                || contentType?.includes("application/problem+json")
            ? JSON.parse(xhr.responseText)
            : xhr.responseText;
    } catch (err) {
        reject(new ApiError(xhr.status, err instanceof Error ? err.message : "Unknown error"));
        return;
    }

    try {
        const result = requestHelper.afterRequest(xhr.status === 200, xhr.status, json);
        resolve(result as (T | null));
    } catch (err) {
        reject(err);
    }
}

export const fileUploadHelper = {
    async post<T>(
        relativeUrl: string,
        params: Param[] | null,
        files: FileList | File,
        progressCallback?: (percent: number) => void
    ): Promise<T | null> {

        const formData = createFormData(files);

        const url = requestHelper.beforeRequest(relativeUrl, params);
        if (!url) {
            return null;
        }

        return new Promise<T | null>((resolve, reject) => {
            const xhr = new XMLHttpRequest();

            xhr.open("POST", url, true);
            xhr.setRequestHeader("Accept", "*/*");
            xhr.withCredentials = true;

            xhr.upload.addEventListener("progress", (e) => {
                if (e.lengthComputable) {
                    const percentComplete = Math.round((e.loaded / e.total) * 100);
                    progressCallback?.(percentComplete);
                }
            });

            xhr.addEventListener("load", (e) => onLoad(e.target as XMLHttpRequest, resolve, reject));

            xhr.addEventListener("error", () => {
                reject(new ApiError(xhr.status, "File upload failed due to a network error."));
            });

            xhr.send(formData);
        });
    }
};