import { ApiDashboardNotification } from "@/api/controllers/platform/dashboard/ApiDashboardNotification";
import { ApiDashboardNotificationType } from "@/api/controllers/platform/dashboard/ApiDashboardNotificationType";
import { shiftClient } from "@/api/shiftClient";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTime } from "@/helpers/date/dateTimeTypes";
import { TimeZoneId } from "@/helpers/date/timeZones";

export interface NotificationFormValues {
    type: ApiDashboardNotificationType;
    title: string;
    details: string;
    linkText: string;
    linkUrl: string;
    startDate: DateTime;
    endDate: DateTime;
    isActive: "yes" | "no";
}

export function toApiNotificationFormValues(notificationId: string | null, values: NotificationFormValues): Parameters<typeof shiftClient.dashboard.modifyNotification>[0] {
    return {
        NotificationId: notificationId,
        Type: values.type,
        Title: values.title,
        Details: values.details,
        LinkText: values.linkText,
        LinkUrl: values.linkUrl,
        StartDate: dateTimeHelper.formatServerDateTime(values.startDate),
        EndDate: dateTimeHelper.formatServerDateTime(values.endDate),
        IsActive: values.isActive === "yes",
    }
}

export function fromApiNotification(model: ApiDashboardNotification, timeZoneId: TimeZoneId): NotificationFormValues {
    return {
        type: model.Type,
        title: model.Title,
        details: model.Details ?? "",
        linkText: model.LinkText ?? "",
        linkUrl: model.LinkUrl ?? "",
        startDate: dateTimeHelper.parseServerDateTime(model.StartDate, timeZoneId),
        endDate: dateTimeHelper.parseServerDateTime(model.EndDate, timeZoneId),
        isActive: model.IsActive ? "yes" : "no",
    }
}