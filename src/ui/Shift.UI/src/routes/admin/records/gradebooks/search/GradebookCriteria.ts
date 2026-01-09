import { ApiSearchGradebooks } from "@/api/controllers/gradebook/ApiSearchGradebooks";
import { BaseCriteria } from "@/components/search/BaseCriteria";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTime } from "@/helpers/date/dateTimeTypes";

export interface GradebookCriteria extends BaseCriteria {
    gradebookTitle: string;
    gradebookCreatedSince: DateTime;
    gradebookCreatedBefore: DateTime;
    periodIdentifier: string;
    achievementIdentifier: string;
    frameworkIdentifier: string;
    gradebookStatus: "locked" | "unlocked" | "";
    classTitle: string;
    classStartedSince: DateTime;
    classStartedBefore: DateTime;
    classInstructorIdentifier: string;
}

export function defaultGradebookCriteria(): GradebookCriteria {
    return {
        gradebookTitle: "",
        gradebookCreatedSince: null,
        gradebookCreatedBefore: null,
        periodIdentifier: "",
        achievementIdentifier: "",
        frameworkIdentifier: "",
        gradebookStatus: "",
        classTitle: "",
        classStartedSince: null,
        classStartedBefore: null,
        classInstructorIdentifier: "",
        visibleColumns: [],
        sortByColumn: ""
    }
}

export function toApiSearchGradebooks(criteria: GradebookCriteria): ApiSearchGradebooks {
    return {
        GradebookTitle: criteria.gradebookTitle,
        GradebookCreatedSince: dateTimeHelper.formatServerDateTime(criteria.gradebookCreatedSince),
        GradebookCreatedBefore: dateTimeHelper.formatServerDateTime(criteria.gradebookCreatedBefore),
        PeriodIdentifier: criteria.periodIdentifier,
        AchievementIdentifier: criteria.achievementIdentifier,
        FrameworkIdentifier: criteria.frameworkIdentifier,
        IsLocked: criteria.gradebookStatus === "locked" ? true : (criteria.gradebookStatus === "unlocked" ? false : null),
        ClassTitle: criteria.classTitle,
        ClassStartedSince: dateTimeHelper.formatServerDateTime(criteria.classStartedSince),
        ClassStartedBefore: dateTimeHelper.formatServerDateTime(criteria.classStartedBefore),
        ClassInstructorIdentifier: criteria.classInstructorIdentifier,
    }
}