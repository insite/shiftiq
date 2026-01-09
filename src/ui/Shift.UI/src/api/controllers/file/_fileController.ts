import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { ApiCountResult } from "@/api/models/ApiCountResult";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchFiles } from "./ApiSearchFiles";
import { ApiFileMatch } from "./ApiFileMatch";
import { ApiFileModel } from "./ApiFileModel";
import { ApiUploadFileInfo } from "./ApiUploadFileInfo";
import { fileUploadHelper } from "@/api/fileUploadHelper";

const _baseUrl = "/content/files";

export const _fileController = {
    async count(query: ApiSearchFiles) {
        const result = await fetchHelper.get<ApiCountResult>(`${_baseUrl}/count`, fetchHelper.toParams(query));
        return result.Count;
    },

    async search(query: ApiSearchFiles, pageIndex: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiFileMatch>(`${_baseUrl}/search`, query, pageIndex, null, sortByColumn, null);
    },

    async download(query: ApiSearchFiles, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`${_baseUrl}/download`, query, format, visibleColumns);
    },

    async retrieve(fileId: string) {
        return await fetchHelper.get<ApiFileModel>(`${_baseUrl}/${fileId}`);
    },

    async uploadTempFile(files: FileList | File, responseId?: string | null, progressCallback?: (percent: number) => void): Promise<ApiUploadFileInfo[] | null> {
        return await fileUploadHelper.post(`${_baseUrl}/temp`, [{ name: "responseId", value: responseId }], files, progressCallback);
    }
}