import { translate } from "../translate";

export type TimeZoneId =
    "Pacific/Honolulu"
    | "America/Anchorage"
    | "America/Vancouver"
    | "America/Edmonton"
    | "America/Winnipeg"
    | "America/Toronto"
    | "America/St_Johns"
    | "America/Halifax"
    | "UTC";

export interface TimeZoneInfo {
    readonly timeZoneId: TimeZoneId;
    readonly title: string;
    readonly abbrev: string;
    readonly standardCode?: string;
    readonly daylightCode?: string;
}

// https://en.wikipedia.org/wiki/List_of_tz_database_time_zones

export const timeZones: TimeZoneInfo[] = [
    { title: translate("(UTC-10:00) Hawaii"), timeZoneId: "Pacific/Honolulu", abbrev: "HST" },
    { title: translate("(UTC-09:00) Alaska"), timeZoneId: "America/Anchorage", abbrev: "AKT", standardCode: "AKST", daylightCode: "AKDT" },
    { title: translate("(UTC-08:00) Pacific Time (US & Canada)"), timeZoneId: "America/Vancouver", abbrev: "PT", standardCode: "PST", daylightCode: "PDT" },
    { title: translate("(UTC-07:00) Mountain Time (US & Canada)"), timeZoneId: "America/Edmonton", abbrev: "MT", standardCode: "MST", daylightCode: "MDT" },
    { title: translate("(UTC-06:00) Central Time (US & Canada)"), timeZoneId: "America/Winnipeg", abbrev: "CT", standardCode: "CST", daylightCode: "CDT" },
    { title: translate("(UTC-05:00) Eastern Time (US & Canada)"), timeZoneId: "America/Toronto", abbrev: "ET", standardCode: "EST", daylightCode: "EDT" },
    { title: translate("(UTC-03:30) Newfoundland (US & Canada)"), timeZoneId: "America/St_Johns", abbrev: "NT", standardCode: "NST", daylightCode: "NDT" },
    { title: translate("(UTC-04:00) Atlantic Time (US & Canada)"), timeZoneId: "America/Halifax", abbrev: "AT", standardCode: "AST", daylightCode: "ADT" },
    { title: translate("UTC"), timeZoneId: "UTC", abbrev: "UTC" },
];

export const timeZoneIds = {
    UTC: "UTC"
}

export function getTimeZone(timeZoneId: string, throwErrorIfNotFound?: boolean | undefined): TimeZoneInfo | undefined {
    const result = timeZones.find(x => x.timeZoneId === timeZoneId);
    if (!result && throwErrorIfNotFound) {
        throw new Error(`Unknown timezone: ${timeZoneId}`);
    }
    return result;
}
