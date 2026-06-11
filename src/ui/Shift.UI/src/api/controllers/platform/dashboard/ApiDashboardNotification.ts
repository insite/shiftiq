import { ApiDashboardNotificationType } from "./ApiDashboardNotificationType";

export interface ApiDashboardNotification {
    NotificationId: string;
    Type: ApiDashboardNotificationType;
    Title: string;
    Details: string | null | undefined;
    LinkText: string | null | undefined;
    LinkUrl: string | null | undefined;
    StartDate: string | null | undefined;
    EndDate: string | null | undefined;
    IsActive: boolean;
    Created: string;
    CreatedBy: string;
    Modified: string;
    ModifiedBy: string;
}