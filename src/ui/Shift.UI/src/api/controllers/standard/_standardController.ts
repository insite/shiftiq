import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchStandards } from "./ApiSearchStandards";
import { ApiStandardMatch } from "./ApiStandardMatch";
import { ApiStandardModel } from "./ApiStandardModel";

export const _standardController = {
    async search(query: ApiSearchStandards, pageIndex: number, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiStandardMatch>(`/api/competency/standards/search`, query, pageIndex, pageSize, null, null);
    },

    async retrieve(standardId: string) {
        return await fetchHelper.get<ApiStandardModel>(`/api/competency/standards/${standardId}`);
    }
}