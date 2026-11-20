import { setTimeZones } from "@/helpers/date/timeZones";

export function setupShared() {
    setTimeZones([
        { title: "(UTC-10:00) Hawaii", timeZoneId: "Pacific/Honolulu", abbrev: "HST", isIANACompatible: true },
        { title: "(UTC-09:00) Alaska", timeZoneId: "America/Anchorage", abbrev: "AKT", standardCode: "AKST", daylightCode: "AKDT", isIANACompatible: true },
        { title: "(UTC-08:00) Pacific Time (US & Canada)", timeZoneId: "America/Vancouver", abbrev: "PT", standardCode: "PST", daylightCode: "PDT", isIANACompatible: false },
        { title: "(UTC-07:00) Mountain Time (US & Canada)", timeZoneId: "America/Edmonton", abbrev: "MT", standardCode: "MST", daylightCode: "MDT", isIANACompatible: false },
        { title: "(UTC-06:00) Central Time (US & Canada)", timeZoneId: "America/Winnipeg", abbrev: "CT", standardCode: "CST", daylightCode: "CDT", isIANACompatible: true },
        { title: "(UTC-05:00) Eastern Time (US & Canada)", timeZoneId: "America/Toronto", abbrev: "ET", standardCode: "EST", daylightCode: "EDT", isIANACompatible: true },
        { title: "(UTC-03:30) Newfoundland (US & Canada)", timeZoneId: "America/St_Johns", abbrev: "NT", standardCode: "NST", daylightCode: "NDT", isIANACompatible: true },
        { title: "(UTC-04:00) Atlantic Time (US & Canada)", timeZoneId: "America/Halifax", abbrev: "AT", standardCode: "AST", daylightCode: "ADT", isIANACompatible: true },
        { title: "UTC", timeZoneId: "UTC", abbrev: "UTC", isIANACompatible: true },
    ]);
}