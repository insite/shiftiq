import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchGradebooks } from "./ApiSearchGradebooks";
import { ApiGradebookMatch } from "./ApiGradebookMatch";
import { ApiGradebookModel } from "./ApiGradebookModel";

const _baseUrl = "/progress/gradebooks";

export const _gradebookController = {
    async search(query: ApiSearchGradebooks, pageIndex: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiGradebookMatch>(`${_baseUrl}/search`, query, pageIndex, null, sortByColumn, null);
    },

    async download(query: ApiSearchGradebooks, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`${_baseUrl}/download`, query, format, visibleColumns);
    },

    async retrieve(gradebookId: string) {
        return await fetchHelper.get<ApiGradebookModel>(`${_baseUrl}/${gradebookId}`);
    }
}