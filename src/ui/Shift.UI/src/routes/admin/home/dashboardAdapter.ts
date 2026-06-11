import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { TimeZoneId } from "@/helpers/date/timeZones";
import { ApiDashboard } from "@/api/controllers/platform/dashboard/ApiDashboard";
import { Dashboard } from "./Dashboard";

export const dashboardAdapter = {
    getDashboard(dashboard: ApiDashboard, timeZoneId: TimeZoneId): Dashboard {
        const activeNotifications = dashboard.ActiveNotifications.map(x => ({
            notificationId: x.NotificationId,
            type: x.Type,
            title: x.Title,
            details: x.Details ?? null,
            linkText: x.LinkText ?? null,
            linkUrl: x.LinkUrl ?? null,
            modified: dateTimeHelper.parseServerDateTime(x.Modified, timeZoneId)!,
        }));

        const apiCounts = dashboard.Counts;

        const counts = {
            bankCount: apiCounts.BankCount,
            activeBankCount: apiCounts.ActiveBankCount,
            formCount: apiCounts.FormCount,
            publishedFormCount: apiCounts.PublishedFormCount,
            personCount: apiCounts.PersonCount,
            activePersonCount: apiCounts.ActivePersonCount,
            approvedPersonCount: apiCounts.ApprovedPersonCount,
            courseCount: apiCounts.CourseCount,
            publishedCourseCount: apiCounts.PublishedCourseCount,
            startedEnrollmentCount: apiCounts.StartedEnrollmentCount,
            completedEnrollmentCount: apiCounts.CompletedEnrollmentCount,
        };

        return {
            activeNotifications,
            hideMyDashboard: dashboard.HideMyDashboard,
            counts
        };
    }
}