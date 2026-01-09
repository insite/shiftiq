import { ApiGradebookMatch } from "@/api/controllers/gradebook/ApiGradebookMatch";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeParts } from "@/helpers/date/dateTimeTypes";
import { TimeZoneId } from "@/helpers/date/timeZones";

export interface GradebookRow {
    gradebookId: string;
    gradebookTitle: string;
    gradebookCreated: DateTimeParts;
    gradebookEnrollmentCount: number;
    classId: string | null;
    classTitle: string | null;
    classStarted: DateTimeParts | null;
    classEnded: DateTimeParts | null;
    achievementId: string | null;
    achievementTitle: string | null;
    achievementCountGranted: number;
    isLocked: boolean;
}

export function toGradebookRow(row: ApiGradebookMatch, timeZoneId: TimeZoneId) {
    return {
        gradebookId: row.GradebookIdentifier,
        gradebookTitle: row.GradebookTitle,
        gradebookCreated: dateTimeHelper.parseServerDateTime(row.GradebookCreated, timeZoneId)!,
        gradebookEnrollmentCount: row.GradebookEnrollmentCount,
        classId: row.ClassIdentifier ?? null,
        classTitle: row.ClassTitle ?? null,
        classStarted: dateTimeHelper.parseServerDateTime(row.ClassStarted, timeZoneId),
        classEnded: dateTimeHelper.parseServerDateTime(row.ClassEnded, timeZoneId),
        achievementId: row.AchievementIdentifier ?? null,
        achievementTitle: row.AchievementTitle ?? null,
        achievementCountGranted: row.AchievementCountGranted,
        isLocked: row.IsLocked,
    };
}