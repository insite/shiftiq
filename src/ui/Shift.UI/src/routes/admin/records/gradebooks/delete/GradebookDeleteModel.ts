import { DateTimeParts } from "@/helpers/date/dateTimeTypes";

export interface GradebookDeleteModel {
    gradebookId: string;
    gradebookTitle: string;
    gradebookType: string;
    created: DateTimeParts;

    achievementId: string | null;
    achievementTitle: string | null;

    eventId: string | null;
    eventTitle: string | null;
    eventScheduledStart: DateTimeParts | null;
    eventScheduledEnd: DateTimeParts | null;

    consequences: {
        name: string;
        count: number;
    }[];
}