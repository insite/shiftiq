import { ApiDashboardNotification } from "@/api/controllers/platform/dashboard/ApiDashboardNotification";
import { ApiDashboardNotificationType } from "@/api/controllers/platform/dashboard/ApiDashboardNotificationType";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { TimeZoneId } from "@/helpers/date/timeZones";

export interface NotificationRow {
    notificationId: string;
    type: ApiDashboardNotificationType;
    title: string;
    details: string | null;
    linkText: string | null;
    linkUrl: string | null;
    startDate: DateTimeParts | null;
    endDate: DateTimeParts | null;
    isActive: boolean;
    created: DateTimeParts;
    createdByName: string;
    modified: DateTimeParts;
    modifiedByName: string;
    visibleOnDashboard: boolean;
}

export function toNotificationRow(row: ApiDashboardNotification, users: Record<string, string>, visibleOnDashboard: Record<string, boolean>, timeZoneId: TimeZoneId): NotificationRow {
    return {
        notificationId: row.NotificationId,
        type: row.Type,
        title: row.Title,
        details: row.Details ?? null,
        linkText: row.LinkText ?? null,
        linkUrl: row.LinkUrl ?? null,
        startDate: dateTimeHelper.parseServerDateTime(row.StartDate, timeZoneId),
        endDate: dateTimeHelper.parseServerDateTime(row.EndDate, timeZoneId),
        isActive: row.IsActive,
        created: dateTimeHelper.parseServerDateTime(row.Created, timeZoneId)!,
        createdByName: users[row.CreatedBy] ?? "Unknown",
        modified: dateTimeHelper.parseServerDateTime(row.Modified, timeZoneId)!,
        modifiedByName: users[row.ModifiedBy] ?? "Unknown",
        visibleOnDashboard: visibleOnDashboard[row.NotificationId] === true
    };
}