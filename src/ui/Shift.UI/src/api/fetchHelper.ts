import { ApiError } from "./apiError";
import { QueryResult } from "@/models/QueryResult";
import { ApiQueryPagination } from "./models/ApiQueryPagination";
import { ApiDownload, ApiDownloadFormat } from "./models/ApiDownload";
import { ObjectIndexer } from "@/models/ObjectIndexer";
import { Param, requestHelper } from "./requestHelper";
import { shiftConfig } from "@/helpers/shiftConfig";


async function afterRequest(response: Response, asBlob: true, returnNullOn404: boolean): Promise<Blob>;
async function afterRequest<T>(response: Response, asBlob: false, returnNullOn404: boolean): Promise<T>;
async function afterRequest<T>(response: Response, asBlob: boolean, returnNullOn404: boolean): Promise<T> {
    let json: unknown;
    try {
        json = asBlob
            ? await response.blob()
            : response.headers.get("Content-Type")?.includes("application/json")
                    || response.headers.get("Content-Type")?.includes("application/problem+json")
                ? await response.json()
                : await response.text();
    } catch (err) {
        throw new ApiError(response.status, err instanceof Error ? err.message : "Unknown error");
    }

    // Let's add a little bit latency for local environment after all requests are completed
    if (process.env.NODE_ENV === "development" && shiftConfig.localEnvApiLatency > 0) {
        await new Promise(resolve => setTimeout(resolve, shiftConfig.localEnvApiLatency));
    }

    if (response.status === 404 && returnNullOn404) {
        return null as T;
    }

    return requestHelper.afterRequest(response.ok, response.status, json) as T;
}

function getPagination(response: Response) {
    const xQueryPagination = response.headers.get("X-Query-Pagination");
    if (!xQueryPagination) {
        throw new Error("X-Query-Pagination is empty");
    }

    const pagination = JSON.parse(xQueryPagination) as ApiQueryPagination;
    if (pagination.Page === undefined || pagination.PageSize === undefined || pagination.TotalCount === undefined) {
        throw new Error("X-Query-Pagination is invalid");
    }

    return pagination;
}

function removeEmptyProps(obj: object) {
    const result: ObjectIndexer = {};

    for (const key in obj) {
        const value = (obj as ObjectIndexer)[key];
        if (value !== null && value !== undefined && value !== "") {
            result[key] = value;
        }
    }

    return result as object;
}

export const fetchHelper = {
    async getPagedRows<Row extends object>(
        relativeUrl: string,
        query: object,
        pageIndex: number,
        pageSize: number | null,
        sortByColumn: string | null,
        visibleColumns: string[] | null,
    ): Promise<QueryResult<Row> | null> {
        const url = requestHelper.beforeRequest(relativeUrl, null);
        if (!url) {
            return null;
        }

        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({
                ...removeEmptyProps(query),
                Filter: {
                    Page: pageIndex + 1,
                    PageSize: pageSize ?? undefined,
                    Sort: sortByColumn ?? undefined,
                    Includes: visibleColumns?.length ? visibleColumns.join(",") : undefined,
                }
            }),
            headers: {
                "Accept": "*/*",
                "Content-Type": "application/json",
            },
            credentials: "include"
        });

        const rows = await afterRequest<Row[]>(response, false, false);
        if (!rows) {
            return null;
        }

        const pagination = getPagination(response);

        return {
            pageIndex: pagination.Page - 1,
            rows,
            totalRowCount: pagination.TotalCount,
            rowsPerPage: pagination.PageSize
        };
    },

    async download(
        relativeUrl: string,
        query: object,
        format: ApiDownloadFormat,
        visibleColumns: string[],
    ): Promise<ApiDownload | null> {
        const url = requestHelper.beforeRequest(relativeUrl, null);
        if (!url) {
            return null;
        }

        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify({
                ...removeEmptyProps(query),
                Filter: {
                    Format: format,
                    Includes: visibleColumns.length ? visibleColumns.join(",") : undefined,
                }
            }),
            headers: {
                "Accept": "*/*",
                "Content-Type": "application/json",
            },
            credentials: "include"
        });

        const data = await afterRequest(response, true, false);
        if (!data) {
            return null;
        }

        const contentDisposition = response.headers.get("Content-Disposition");
        const filename = contentDisposition ? contentDisposition.split("filename=")[1].split(';')[0] : null;

        return {
            filename: filename ?? `data.${format}`,
            data
        };
    },

    async get<T>(relativeUrl: string, params?: Param[] | null, returnNullOn404?: boolean): Promise<T> {
        const url = requestHelper.beforeRequest(relativeUrl, params);
        if (!url) {
            return null as T;
        }

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "*/*"
            },
            credentials: "include"
        });

        return await afterRequest(response, false, returnNullOn404 === true);
    },

    async post<T>(relativeUrl: string, body: unknown, params?: Param[] | null): Promise<T> {
        const url = requestHelper.beforeRequest(relativeUrl, params);
        if (!url) {
            return null as T;
        }

        const response = await fetch(url, {
            method: "POST",
            body: body ? JSON.stringify(body) : undefined,
            headers: {
                "Accept": "*/*",
                "Content-Type": "application/json",
            },
            credentials: "include"
        });

        return await afterRequest(response, false, false);
    },

    async postForm<T>(relativeUrl: string, body: FormData, params?: Param[] | null): Promise<T> {
        const url = requestHelper.beforeRequest(relativeUrl, params);
        if (!url) {
            return null as T;
        }

        const response = await fetch(url, {
            method: "POST",
            body: body,
            headers: {
                "Accept": "*/*"
            },
            credentials: "include"
        });

        return await afterRequest(response, false, false);
    },

    async put<T>(relativeUrl: string, body: unknown, params?: Param[] | null): Promise<T> {
        const url = requestHelper.beforeRequest(relativeUrl, params);
        if (!url) {
            return null as T;
        }

        const response = await fetch(url, {
            method: "PUT",
            body: body ? JSON.stringify(body) : undefined,
            headers: {
                "Accept": "*/*",
                "Content-Type": "application/json",
            },
            credentials: "include"
        });

        return await afterRequest(response, false, false);
    },

    async delete<T>(relativeUrl: string, body: unknown, params?: Param[] | null): Promise<T> {
        const url = requestHelper.beforeRequest(relativeUrl, params);
        if (!url) {
            return null as T;
        }

        const response = await fetch(url, {
            method: "DELETE",
            body: body ? JSON.stringify(body) : undefined,
            headers: {
                "Accept": "*/*",
                "Content-Type": "application/json",
            },
            credentials: "include"
        });

        return await afterRequest(response, false, false);
    },

    toParams(obj: object | undefined | null): Param[] | null {
        if (!obj) {
            return null;
        }

        const params: Param[] = [];
        for (const key in obj) {
            const value = (obj as ObjectIndexer)[key];
            if (!value) {
                continue;
            }

            params.push({
                name: key,
                value: String(value)
            })
        }

        return params;
    }
}