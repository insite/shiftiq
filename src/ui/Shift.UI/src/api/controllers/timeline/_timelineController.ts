import { fetchHelper } from "@/api/fetchHelper";

export const _timelineController = {
    async retrieve(aggregateId: string, aggregateType: "BankAggregate"): Promise<string | null> {
        const params = [{ name: "aggregateType", value: aggregateType }];
        return await fetchHelper.get<unknown>(`/api/timeline/aggregates/${aggregateId}`, params, true) as string;
    }
}