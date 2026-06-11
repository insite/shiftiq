import { ApiDashboardNotificationType } from "@/api/controllers/platform/dashboard/ApiDashboardNotificationType";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";

export interface ActiveNotification {
    notificationId: string;
    type: ApiDashboardNotificationType;
    title: string;
    details: string | null;
    linkText: string | null;
    linkUrl: string | null;
    modified: DateTimeParts;
}