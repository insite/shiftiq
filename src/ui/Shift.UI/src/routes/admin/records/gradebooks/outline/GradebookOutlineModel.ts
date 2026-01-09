import { DateTimeParts } from "@/helpers/date/dateTimeTypes";

export interface GradebookOutlineModel {
    gradebookId: string;
    gradebookTitle: string;
    gradebookType: string;
    isLocked: boolean;

    achievementId: string | null;
    achievementTitle: string | null;

    eventId: string | null;
    eventTitle: string | null;
    eventScheduledStart: DateTimeParts | null;
    eventScheduledEnd: DateTimeParts | null;

    periodId: string | null;
    periodTitle: string | null;

    frameworkId: string | null;
    frameworkTitle: string | null;

    reference: string | null;
}