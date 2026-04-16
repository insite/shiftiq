import { WorkshopFlag } from "@/contexts/workshop/models/WorkshopEnums";
import { WorkshopQuestion } from "@/contexts/workshop/models/WorkshopQuestion";
import { WorkshopQuestionChangeDates } from "@/contexts/workshop/models/WorkshopQuestionChangeDates";
import { base64Helper } from "@/helpers/base64Helper";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateOrDateTime, DateParts, isDate } from "@/helpers/date/dateTimeTypes";
import { TimeZoneId } from "@/helpers/date/timeZones";

export type WorkshopQuestionRange = "" | "Today" | "Yesterday" | "ThisWeek" | "LastWeek" | "ThisMonth" | "LastMonth" | "ThisYear" | "LastYear" | "Custom";

export interface WorkshopQuestionFilterState {
    sectionId: string | null;
    competencyId: string | null;
    flags: WorkshopFlag[];
    conditions: string[];
    taxonomy: number | null;
    hasLig: boolean | null;
    hasReference: boolean | null;
    changedOn: WorkshopQuestionRange;
    changedOnSince: DateOrDateTime;
    changedOnBefore: DateOrDateTime;
}

interface WorkshopQuestionRangeDates {
    rangeSince: Date | null;
    rangeBefore: Date | null;
}

function getRangeDates(range: WorkshopQuestionRange, since: DateOrDateTime | null, before: DateOrDateTime | null, timeZoneId: TimeZoneId) : WorkshopQuestionRangeDates | null {
    if (!range || range === "Custom" && !isDate(since) && !isDate(before)) {
        return null;
    }
    switch (range) {
        case "Today":
            since = dateTimeHelper.today();
            before = dateTimeHelper.addDays(since, 1);
            break;
        case "Yesterday":
            since = dateTimeHelper.addDays(dateTimeHelper.today(), -1);
            before = dateTimeHelper.addDays(since, 1);
            break;
        case "ThisWeek":
            since = dateTimeHelper.firstWeekDay();
            before = dateTimeHelper.addDays(since, 7);
            break;
        case "LastWeek":
            since = dateTimeHelper.addDays(dateTimeHelper.firstWeekDay(), -7);
            before = dateTimeHelper.addDays(since, 7);
            break;
        case "ThisMonth":
            since = dateTimeHelper.firstMonthDay();
            before = dateTimeHelper.addMonths(since, 1);
            break;
        case "LastMonth":
            since = dateTimeHelper.addMonths(dateTimeHelper.firstMonthDay(), -1);
            before = dateTimeHelper.addMonths(since, 1);
            break;
        case "ThisYear":
            since = dateTimeHelper.firstYearDay();
            before = dateTimeHelper.addMonths(since, 12);
            break;
        case "LastYear":
            before = dateTimeHelper.firstYearDay();
            since = dateTimeHelper.addMonths(before, -12);
            break;
        case "Custom":
            if (before) {
                before = dateTimeHelper.addDays(before as DateParts, 1);
            }
            since = since as DateParts;
            break;
        default:
            throw new Error(`Invalid range: ${range}`);
    }

    const result = {
        rangeSince: since ? dateTimeHelper.toUtcDate(since, timeZoneId) : null,
        rangeBefore: before ? dateTimeHelper.toUtcDate(before, timeZoneId) : null,
    };

    return result;
}

function isQuestionMatch(
    question: WorkshopQuestion,
    filter: WorkshopQuestionFilterState,
    rangeDates: WorkshopQuestionRangeDates | null,
    questionChangeDates: WorkshopQuestionChangeDates | null,
): boolean
{
    if (question.standardId !== (filter.competencyId || null)) {
        return false;
    }

    if (filter.flags.length > 0 && !filter.flags.includes(question.questionFlag)) {
        return false;
    }

    if (filter.conditions.length > 0 && !filter.conditions.includes(getQuestionCondition(question))) {
        return false;
    }

    if (filter.taxonomy !== null && question.questionTaxonomy !== filter.taxonomy) {
        return false;
    }

    if (filter.hasLig !== null && hasText(question.questionLikeItemGroup) !== filter.hasLig) {
        return false;
    }

    if (filter.hasReference !== null && hasText(question.questionReference) !== filter.hasReference) {
        return false;
    }

    if (rangeDates && questionChangeDates && !isQuestionDateMatch(rangeDates, questionChangeDates, question.questionId)) {
        return false;
    }

    return true;
}

function isQuestionDateMatch({ rangeSince, rangeBefore }: WorkshopQuestionRangeDates, questionChangeDates: WorkshopQuestionChangeDates, questionId: string): boolean {
    const date = questionChangeDates[questionId];
    return date && (!rangeSince || date >= rangeSince) && (!rangeBefore || date < rangeBefore);
}

function getQuestionCondition(question: WorkshopQuestion): string {
    return question.questionCondition?.trim() || "Unassigned";
}

function hasText(value: string | null): boolean {
    return !!value?.trim();
}

export const workshopQuestionFilter = {
    createWorkshopQuestionsFilterState(sectionId: string | null, competencyId: string | null): WorkshopQuestionFilterState {
        return {
            sectionId,
            competencyId,
            flags: [],
            conditions: [],
            taxonomy: null,
            hasLig: null,
            hasReference: null,
            changedOn: "",
            changedOnSince: null,
            changedOnBefore: null,
        };
    },

    filterQuestions(
        questions: WorkshopQuestion[] | null,
        filter: WorkshopQuestionFilterState,
        questionChangeDates: WorkshopQuestionChangeDates | null,
        timeZoneId: TimeZoneId
    ): WorkshopQuestion[] {
        if (!questions) {
            return [];
        }

        const rangeDates = getRangeDates(filter.changedOn, filter.changedOnSince, filter.changedOnBefore, timeZoneId);
        if (rangeDates && !questionChangeDates) {
            throw new Error("questionChangeDates is null");
        }

        return questions.filter(question => isQuestionMatch(question, filter, rangeDates, questionChangeDates));
    },

    serializeFilter(filter: WorkshopQuestionFilterState): string {
        return base64Helper.objectToBase64(filter);
    },

    deserializeFilter(base64: string): WorkshopQuestionFilterState {
        return base64Helper.base64ToObject(base64);
    },

    hasDateFilter(filter: WorkshopQuestionFilterState): boolean {
        return !!getRangeDates(filter.changedOn, filter.changedOnSince, filter.changedOnBefore, "UTC");
    },
}