import { fetchHelper } from "@/api/fetchHelper";

export const _toolkitUsageController = {
    async visit(actionUrl: string): Promise<void> {
        await fetchHelper.post("/api/reports/toolkit-usage/visit", { actionUrl });
    }
}