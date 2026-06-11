import { ApiDashboardNotificationCriteria } from "@/api/controllers/platform/dashboard/ApiDashboardNotificationCriteria";
import { BaseCriteria } from "@/components/search/BaseCriteria";

export interface NotificationCriteria extends BaseCriteria {
    title: string;
    onlyVisibleOnDashboard: "yes" | "no";
}

export function defaultNotificationCriteria(): NotificationCriteria {
    return {
        title: "",
        onlyVisibleOnDashboard: "no",
        visibleColumns: [],
        sortByColumn: ""
    }
}

export function toApiDashboardNotificationCriteria(criteria: NotificationCriteria): ApiDashboardNotificationCriteria {
    return {
        Title: criteria.title,
        OnlyVisibleOnDashboard: criteria.onlyVisibleOnDashboard === "yes",
    }
}