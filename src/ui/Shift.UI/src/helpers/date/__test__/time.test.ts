import { expect, test } from "vitest";
import { _dateTimeParser } from "../_dateTimeParser";
import { dateTimeHelper } from "../dateTimeHelper";

test("_dateTimeParser.parsers  - h:m tt z", () => {
    expect(_dateTimeParser.timeParsers["h:m tt z"](["5", "15", "pm", "mt"])).toEqual({ hour: 17, minute: 15, timeZoneId: "America/Edmonton" });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["5", "15", "pm", "mst"])).toEqual({ hour: 17, minute: 15, timeZoneId: "America/Edmonton" });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["5", "15", "pm", "mdt"])).toEqual({ hour: 17, minute: 15, timeZoneId: "America/Edmonton" });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3", "0", "am", "pt"])).toEqual({ hour: 3, minute: 0, timeZoneId: "America/Vancouver" });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3", "0", "am", "pst"])).toEqual({ hour: 3, minute: 0, timeZoneId: "America/Vancouver" });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3", "0", "am", "pdt"])).toEqual({ hour: 3, minute: 0, timeZoneId: "America/Vancouver" });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3", "55", "am"])).toEqual({ hour: 3, minute: 55, timeZoneId: null });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3", "55"])).toEqual({ hour: 3, minute: 55, timeZoneId: null });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["12", "55", "am"])).toEqual({ hour: 0, minute: 55, timeZoneId: null });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["12", "55", "pm"])).toEqual({ hour: 12, minute: 55, timeZoneId: null });
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3"])).toEqual(null);
    expect(_dateTimeParser.timeParsers["h:m tt z"](["0", "55"])).toEqual(null);
    expect(_dateTimeParser.timeParsers["h:m tt z"](["13", "55"])).toEqual(null);
    expect(_dateTimeParser.timeParsers["h:m tt z"](["3", "60"])).toEqual(null);
});

test("dateHelper.parseDateTime", () => {
    expect(dateTimeHelper.parseDateTime("Apr 15, 2025")).toEqual({ date: { day: 15, month: 4, year: 2025 }, time: { hour: 0, minute: 0, timeZoneId: "UTC" } });
    expect(dateTimeHelper.parseDateTime("Apr 15, 2025 12:14 AM MT")).toEqual({ date: { day: 15, month: 4, year: 2025 }, time: { hour: 0, minute: 14, timeZoneId: "America/Edmonton" } });
    expect(dateTimeHelper.parseDateTime("Apr 15, 2025 10:14 AM MDT")).toEqual({ date: { day: 15, month: 4, year: 2025 }, time: { hour: 10, minute: 14, timeZoneId: "America/Edmonton" } });
    expect(dateTimeHelper.parseDateTime("Apr 15, 2025 10:14 PM MST")).toEqual({ date: { day: 15, month: 4, year: 2025 }, time: { hour: 22, minute: 14, timeZoneId: "America/Edmonton" } });
    expect(dateTimeHelper.parseDateTime("Apr 15, 2025 10:14", "America/Halifax")).toEqual({ date: { day: 15, month: 4, year: 2025 }, time: { hour: 10, minute: 14, timeZoneId: "America/Halifax" } });
    expect(dateTimeHelper.parseDateTime("")).toEqual(null);
    expect(dateTimeHelper.parseDateTime(undefined)).toEqual(null);
    expect(dateTimeHelper.parseDateTime(null)).toEqual(null);
});

test("dateHelper.formatDateTime", () => {
    expect(dateTimeHelper.formatDateTime({
        date: { day: 15, month: 4, year: 2025 },
        time: { hour: 15, minute: 45, timeZoneId: "America/Edmonton" }
    }, "mmm d, yyyy")).toEqual("Apr 15, 2025 3:45 PM MDT");

    expect(dateTimeHelper.formatDateTime({
        date: { day: 15, month: 12, year: 2025 },
        time: { hour: 12, minute: 45, timeZoneId: "America/Edmonton" }
    }, "mmm d, yyyy")).toEqual("Dec 15, 2025 12:45 PM MST");

    expect(dateTimeHelper.formatDateTime({
        date: { day: 15, month: 12, year: 2025 },
        time: { hour: 0, minute: 45, timeZoneId: "America/Winnipeg" }
    }, "mmm d, yyyy")).toEqual("Dec 15, 2025 12:45 AM CST");

    expect(dateTimeHelper.formatDateTime({
        date: { day: 15, month: 12, year: 2025 },
        time: { hour: 5, minute: 55, timeZoneId: "America/Vancouver" }
    }, "mmm d, yyyy")).toEqual("Dec 15, 2025 5:55 AM PST");

    expect(dateTimeHelper.formatDateTime({
        date: { day: 0, month: 12, year: 2025 },
        time: { hour: 5, minute: 55, timeZoneId: "America/Vancouver" }
    }, "mmm d, yyyy")).toEqual(null);

    expect(dateTimeHelper.formatDateTime({
        date: { day: 15, month: 12, year: 2025 },
        time: { hour: 24, minute: 55, timeZoneId: "America/Vancouver" }
    }, "mmm d, yyyy")).toEqual(null);
});