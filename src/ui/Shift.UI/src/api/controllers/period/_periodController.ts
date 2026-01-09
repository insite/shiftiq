import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchPeriods } from "./ApiSearchPeriods";
import { ApiPeriodMatch } from "./ApiPeriodMatch";
import { ApiPeriodModel } from "./ApiPeriodModel";

const _baseUrl = "/progress/periods";

export const _periodController = {
    async search(query: ApiSearchPeriods, pageIndex: number, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiPeriodMatch>(`${_baseUrl}/search`, query, pageIndex, pageSize, null, null);
    },

    async retrieve(periodId: string) {
        return await fetchHelper.get<ApiPeriodModel>(`${_baseUrl}/${periodId}`);
    }
}