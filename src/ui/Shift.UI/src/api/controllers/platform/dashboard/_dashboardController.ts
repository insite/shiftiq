import { fetchHelper } from "@/api/fetchHelper";
import { ApiDashboardNotificationCriteria } from "./ApiDashboardNotificationCriteria";
import { QueryResult } from "@/models/QueryResult";
import { ApiDashboardNotification } from "./ApiDashboardNotification";
import { ApiDashboardSearchResult } from "./ApiDashboardSearchResult";
import { ApiDashboardNotificationType } from "./ApiDashboardNotificationType";
import { ApiDashboard } from "./ApiDashboard";

interface ModifiedNotification {
    NotificationId: string | null;
    Type: ApiDashboardNotificationType;
    Title: string;
    Details: string | null;
    LinkText: string | null;
    LinkUrl: string | null;
    StartDate: string | null;
    EndDate: string | null;
    IsActive: boolean;
}

export const _dashboardController = {
    async retrieveDashboard(): Promise<ApiDashboard | null> {
        return await fetchHelper.get<ApiDashboard>("/api/platform/dashboard");
    },

    async searchNotifications(query: ApiDashboardNotificationCriteria, pageIndex: number): Promise<{
        queryResult: QueryResult<ApiDashboardNotification>;
        users: Record<string, string>,
        visibleOnDashboard: Record<string, boolean>,
    } | null>
    {
        const result = await fetchHelper.getPagedRows<object>(`/api/platform/dashboard/notifications/search`, query, pageIndex, null, null, null);
        if (!result) {
            return null;
        }

        const searchResult = result.rows as unknown as ApiDashboardSearchResult;

        return {
            queryResult: {
                pageIndex: result.pageIndex,
                rows: searchResult.Notifications,
                totalRowCount: result.totalRowCount,
                rowsPerPage: result.rowsPerPage
            },
            users: searchResult.Users,
            visibleOnDashboard: searchResult.VisibleOnDashboard,
        }
    },

    async retrieveNotification(notificationId: string): Promise<ApiDashboardNotification | null> {
        return await fetchHelper.get(`/api/platform/dashboard/notifications/${notificationId}`);
    },

    async modifyNotification(notification: ModifiedNotification): Promise<ApiDashboardNotification | null> {
        return await fetchHelper.post("/api/platform/dashboard/notifications", notification);
    },

    async deleteNotification(notificationId: string): Promise<void> {
        await fetchHelper.delete(`/api/platform/dashboard/notifications/${notificationId}`, null);
    },
}