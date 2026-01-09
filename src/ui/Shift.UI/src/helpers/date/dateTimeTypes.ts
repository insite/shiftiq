import { translate } from "../translate";
import { TimeZoneId } from "./timeZones";

export interface DateParts {
    day: number | null;
    month: number | null;
    year: number | null
}

export interface TimeParts {
    hour: number | null;
    minute: number | null;
    timeZoneId: TimeZoneId | null;
}

export interface DateTimeParts {
    date: DateParts;
    time: TimeParts;
}

export interface DateTimeInvalid {
    isInvalid: true;
    text: string;
}

export type DateTime = DateTimeParts | DateTimeInvalid | null;
export type DateOrDateTime = DateParts | DateTimeParts | DateTimeInvalid | null;

export type DateFormatType = "mm/dd/yyyy" | "mmm d, yyyy" | "yyyy-mm-dd";
export type TimeFormatType = "h:m tt z";

export const defaultFormatType: DateFormatType = "mmm d, yyyy";

export const months = [
    { number: 1, name: translate("January"), short: translate("Jan") },
    { number: 2, name: translate("February"), short: translate("Feb") },
    { number: 3, name: translate("March"), short: translate("Mar") },
    { number: 4, name: translate("April"), short: translate("Apr") },
    { number: 5, name: translate("May"), short: translate("May") },
    { number: 6, name: translate("June"), short: translate("Jun") },
    { number: 7, name: translate("July"), short: translate("Jul") },
    { number: 8, name: translate("August"), short: translate("Aug") },
    { number: 9, name: translate("September"), short: translate("Sep") },
    { number: 10, name: translate("October"), short: translate("Oct") },
    { number: 11, name: translate("November"), short: translate("Nov") },
    { number: 12, name: translate("December"), short: translate("Dec") },
];

export function isDateTimeInvalid(value: DateOrDateTime | undefined): value is DateTimeInvalid {
    return !!value && "isInvalid" in value && value.isInvalid;
}

export function isDateTime(value: DateOrDateTime | undefined): value is DateTimeParts {
    return !!value && "date" in value && "time" in value;
}