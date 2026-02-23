import { ApiSearchGradebooks } from "@/api/controllers/gradebook/ApiSearchGradebooks";
import { BaseCriteria } from "@/components/search/BaseCriteria";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTime } from "@/helpers/date/dateTimeTypes";

export interface GradebookCriteria extends BaseCriteria {
    gradebookTitle: string;
    gradebookCreatedSince: DateTime;
    gradebookCreatedBefore: DateTime;
    periodId: string;
    achievementId: string;
    frameworkId: string;
    gradebookStatus: "locked" | "unlocked" | "";
    classTitle: string;
    classStartedSince: DateTime;
    classStartedBefore: DateTime;
    classInstructorId: string;
}

export function defaultGradebookCriteria(): GradebookCriteria {
    return {
        gradebookTitle: "",
        gradebookCreatedSince: null,
        gradebookCreatedBefore: null,
        periodId: "",
        achievementId: "",
        frameworkId: "",
        gradebookStatus: "",
        classTitle: "",
        classStartedSince: null,
        classStartedBefore: null,
        classInstructorId: "",
        visibleColumns: [],
        sortByColumn: ""
    }
}

export function toApiSearchGradebooks(criteria: GradebookCriteria): ApiSearchGradebooks {
    return {
        GradebookTitle: criteria.gradebookTitle,
        GradebookCreatedSince: dateTimeHelper.formatServerDateTime(criteria.gradebookCreatedSince),
        GradebookCreatedBefore: dateTimeHelper.formatServerDateTime(criteria.gradebookCreatedBefore),
        PeriodId: criteria.periodId,
        AchievementId: criteria.achievementId,
        FrameworkId: criteria.frameworkId,
        IsLocked: criteria.gradebookStatus === "locked" ? true : (criteria.gradebookStatus === "unlocked" ? false : null),
        ClassTitle: criteria.classTitle,
        ClassStartedSince: dateTimeHelper.formatServerDateTime(criteria.classStartedSince),
        ClassStartedBefore: dateTimeHelper.formatServerDateTime(criteria.classStartedBefore),
        ClassInstructorId: criteria.classInstructorId,
    }
}