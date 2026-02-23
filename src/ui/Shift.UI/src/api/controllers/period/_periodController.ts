import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchPeriods } from "./ApiSearchPeriods";
import { ApiPeriodMatch } from "./ApiPeriodMatch";
import { ApiPeriodModel } from "./ApiPeriodModel";

export const _periodController = {
    async search(query: ApiSearchPeriods, pageIndex: number, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiPeriodMatch>(`/api/progress/periods/search`, query, pageIndex, pageSize, null, null);
    },

    async retrieve(periodId: string) {
        return await fetchHelper.get<ApiPeriodModel>(`/api/progress/periods/${periodId}`);
    }
}