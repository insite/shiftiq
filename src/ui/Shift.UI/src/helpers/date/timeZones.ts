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
    readonly isIANACompatible: boolean;
}

let timeZones: TimeZoneInfo[] = [{ title: "UTC", timeZoneId: "UTC", abbrev: "UTC", isIANACompatible: true }];

export function getTimeZones(): TimeZoneInfo[] {
    return timeZones;
}

export function setTimeZones(newTimeZones: TimeZoneInfo[]): void {
    timeZones = newTimeZones;
}