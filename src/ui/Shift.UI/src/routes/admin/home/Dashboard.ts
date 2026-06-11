import { ActiveNotification } from "./ActiveNotification";

export interface Dashboard {
    activeNotifications: ActiveNotification[];
    hideMyDashboard: boolean;
    counts: {
        bankCount: number;
        activeBankCount: number;
        formCount: number;
        publishedFormCount: number;
        personCount: number;
        activePersonCount: number;
        approvedPersonCount: number;
        courseCount: number;
        publishedCourseCount: number;
        startedEnrollmentCount: number;
        completedEnrollmentCount: number;
    },
}