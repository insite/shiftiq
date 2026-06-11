import { ApiDashboardNotificationType } from "./ApiDashboardNotificationType";

export interface ApiDashboard {
    ActiveNotifications: {
        NotificationId: string;
        Type: ApiDashboardNotificationType;
        Title: string;
        Details: string | null | undefined;
        LinkText: string | null | undefined;
        LinkUrl: string | null | undefined;
        Modified: string;
    }[];
    HideMyDashboard: boolean;
    Counts: {
        BankCount: number,
        ActiveBankCount: number,
        FormCount: number,
        PublishedFormCount: number,
        PersonCount: number,
        ActivePersonCount: number,
        ApprovedPersonCount: number,
        CourseCount: number,
        PublishedCourseCount: number,
        StartedEnrollmentCount: number,
        CompletedEnrollmentCount: number,
    },
}