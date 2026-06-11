import { fetchHelper } from "@/api/fetchHelper";
import { ApiLockoutResult } from "./ApiLockoutResult";

export const _maintenanceController = {
    maintenanceLockout(): Promise<ApiLockoutResult | null> {
        return fetchHelper.get<ApiLockoutResult>("/api/platform/maintenance/lockout");
    }
}