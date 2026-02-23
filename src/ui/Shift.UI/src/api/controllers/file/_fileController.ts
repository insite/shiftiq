import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { ApiCountResult } from "@/api/models/ApiCountResult";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchFiles } from "./ApiSearchFiles";
import { ApiFileMatch } from "./ApiFileMatch";
import { ApiFileModel } from "./ApiFileModel";
import { ApiUploadFileInfo } from "./ApiUploadFileInfo";
import { fileUploadHelper } from "@/api/fileUploadHelper";

export const _fileController = {
    async count(query: ApiSearchFiles) {
        const result = await fetchHelper.get<ApiCountResult>(`/api/content/files/count`, fetchHelper.toParams(query));
        return result.Count;
    },

    async search(query: ApiSearchFiles, pageIndex: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiFileMatch>(`/api/content/files/search`, query, pageIndex, null, sortByColumn, null);
    },

    async download(query: ApiSearchFiles, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`/api/content/files/download`, query, format, visibleColumns);
    },

    async retrieve(fileId: string) {
        return await fetchHelper.get<ApiFileModel>(`/api/content/files/${fileId}`);
    },

    async uploadTempFile(files: FileList | File, responseId?: string | null, progressCallback?: (percent: number) => void): Promise<ApiUploadFileInfo[] | null> {
        return await fileUploadHelper.post(`/api/content/files/temp`, [{ name: "responseId", value: responseId }], files, progressCallback);
    }
}