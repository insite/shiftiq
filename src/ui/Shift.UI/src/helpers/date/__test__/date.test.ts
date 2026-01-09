import { expect, test } from "vitest";
import { _dateTimeParser } from "../_dateTimeParser";
import { dateTimeHelper } from "../dateTimeHelper";

test("_dateTimeParser.splitText", () => {
    expect(_dateTimeParser.splitText(null)).toEqual(null);
    expect(_dateTimeParser.splitText(undefined)).toEqual(null);
    expect(_dateTimeParser.splitText("")).toEqual(null);
    expect(_dateTimeParser.splitText("/,\\   ")).toEqual(null);
    expect(_dateTimeParser.splitText("12/25/2005")).toEqual({
        date: ["12", "25", "2005"],
        time: [],
    });
    expect(_dateTimeParser.splitText("/12/,/-25, /-2005/  ")).toEqual({
        date: ["12", "25", "2005"],
        time: [],
    });
    expect(_dateTimeParser.splitText("Dec 25, 2005")).toEqual({
        date: ["Dec", "25", "2005"],
        time: [],
    });
    expect(_dateTimeParser.splitText("12/25/2005 13:15")).toEqual({
        date: ["12", "25", "2005"],
        time: ["13", "15"],
    });
    expect(_dateTimeParser.splitText("12/25/2005 13:15 am utc")).toEqual({
        date: ["12", "25", "2005"],
        time: ["13", "15", "am", "utc"],
    });
    expect(_dateTimeParser.splitText("Dec 25, 2005 5:21 pm")).toEqual({
        date: ["Dec", "25", "2005"],
        time: ["5", "21", "pm"],
    });
    expect(_dateTimeParser.splitText("5:21 pm")).toEqual({
        date: [],
        time: ["5", "21", "pm"],
    });
});

test("_dateTimeParser.getDaysInMonth", () => {
    expect(_dateTimeParser.getDaysInMonth(2025, 1)).toEqual(31);
    expect(_dateTimeParser.getDaysInMonth(2025, 2)).toEqual(28);
    expect(_dateTimeParser.getDaysInMonth(2025, 3)).toEqual(31);
    expect(_dateTimeParser.getDaysInMonth(2025, 4)).toEqual(30);
    expect(_dateTimeParser.getDaysInMonth(2025, 5)).toEqual(31);
    expect(_dateTimeParser.getDaysInMonth(2025, 6)).toEqual(30);
    expect(_dateTimeParser.getDaysInMonth(2025, 7)).toEqual(31);
    expect(_dateTimeParser.getDaysInMonth(2025, 8)).toEqual(31);
    expect(_dateTimeParser.getDaysInMonth(2025, 9)).toEqual(30);
    expect(_dateTimeParser.getDaysInMonth(2025, 10)).toEqual(31);
    expect(_dateTimeParser.getDaysInMonth(2025, 11)).toEqual(30);
    expect(_dateTimeParser.getDaysInMonth(2025, 12)).toEqual(31);
});

test("_dateTimeParser.parsers  - mm/dd/yyyy", () => {
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["12", "25", "2025"])).toEqual({ day: 25, month: 12, year: 2025 });
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["12", "25"])).toEqual({ day: 25, month: 12, year: new Date().getFullYear() });
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["02", "28", "2025"])).toEqual({ day: 28, month: 2, year: 2025 });
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["02", "29", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["00", "28", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["15", "28", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["12", "-1", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mm/dd/yyyy"](["12", "28", "20"])).toEqual(null);
});

test("_dateTimeParser.parsers  - yyyy-mm-dd", () => {
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "12", "25"])).toEqual({ day: 25, month: 12, year: 2025 });
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "02", "28"])).toEqual({ day: 28, month: 2, year: 2025 });
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "02"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "02", "29"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "00", "28"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "15", "28"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["2025", "12", "-1"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["yyyy-mm-dd"](["20", "12", "28"])).toEqual(null);
});

test("_dateTimeParser.parsers  - mmm d, yyyy", () => {
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["Dec", "25", "2025"])).toEqual({ day: 25, month: 12, year: 2025 });
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["December", "25"])).toEqual({ day: 25, month: 12, year: new Date().getFullYear() });
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["Feb", "28", "2025"])).toEqual({ day: 28, month: 2, year: 2025 });
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["Feb", "29", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["Feby", "28", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["Dec", "-1", "2025"])).toEqual(null);
    expect(_dateTimeParser.dateParsers["mmm d, yyyy"](["Dec", "28", "20"])).toEqual(null);
});

test("dateHelper.isSeparator", () => {
    expect(dateTimeHelper.isDateSeparator("/")).toEqual(true);
    expect(dateTimeHelper.isDateSeparator("\\")).toEqual(true);
    expect(dateTimeHelper.isDateSeparator("-")).toEqual(true);
    expect(dateTimeHelper.isDateSeparator(" ")).toEqual(true);
    expect(dateTimeHelper.isDateSeparator(",")).toEqual(true);
    expect(dateTimeHelper.isDateSeparator(".")).toEqual(false);
});

test("dateHelper.parseDate", () => {
    expect(dateTimeHelper.parseDate("2025-12-25", "yyyy-mm-dd")).toEqual({ day: 25, month: 12, year: 2025 });
    expect(dateTimeHelper.parseDate("2025-12-25", "mm/dd/yyyy")).toEqual(null);
    expect(dateTimeHelper.parseDate("12/25/2025")).toEqual({ day: 25, month: 12, year: 2025 });
    expect(dateTimeHelper.parseDate("2-27-2025")).toEqual({ day: 27, month: 2, year: 2025 });
    expect(dateTimeHelper.parseDate("Apr 15, 2025")).toEqual({ day: 15, month: 4, year: 2025 });
    expect(dateTimeHelper.parseDate("January 10, 2023")).toEqual({ day: 10, month: 1, year: 2023 });
    expect(dateTimeHelper.parseDate("May 10")).toEqual({ day: 10, month: 5, year: new Date().getFullYear() });
    expect(dateTimeHelper.parseDate("May")).toEqual(null);
    expect(dateTimeHelper.parseDate("Janu 10, 2023")).toEqual(null);
    expect(dateTimeHelper.parseDate("")).toEqual(null);
    expect(dateTimeHelper.parseDate(undefined)).toEqual(null);
    expect(dateTimeHelper.parseDate(null)).toEqual(null);
});

test("dateHelper.formatDate", () => {
    expect(dateTimeHelper.formatDate({ day: 15, month: 4, year: 2025 }, "mmm d, yyyy")).toEqual("Apr 15, 2025");
    expect(dateTimeHelper.formatDate({ day: 25, month: 9, year: 2023 }, "mmm d, yyyy")).toEqual("Sep 25, 2023");
    expect(dateTimeHelper.formatDate({ day: 29, month: 2, year: 2025 }, "mmm d, yyyy")).toEqual(null);
    expect(dateTimeHelper.formatDate(null, "mmm d, yyyy")).toEqual(null);
    expect(dateTimeHelper.formatDate({ day: 15, month: 4, year: 2025 }, "mm/dd/yyyy")).toEqual("04/15/2025");
    expect(dateTimeHelper.formatDate({ day: 25, month: 11, year: 2023 }, "mm/dd/yyyy")).toEqual("11/25/2023");
    expect(dateTimeHelper.formatDate({ day: 29, month: 2, year: 2025 }, "mm/dd/yyyy")).toEqual(null);
    expect(dateTimeHelper.formatDate(null, "mm/dd/yyyy")).toEqual(null);
    expect(dateTimeHelper.formatDate({ day: 15, month: 4, year: 2025 }, "yyyy-mm-dd")).toEqual("2025-04-15");
    expect(dateTimeHelper.formatDate({ day: 25, month: 11, year: 2023 }, "yyyy-mm-dd")).toEqual("2023-11-25");
    expect(dateTimeHelper.formatDate({ day: 29, month: 2, year: 2025 }, "yyyy-mm-dd")).toEqual(null);
    expect(dateTimeHelper.formatDate(null, "yyyy-mm-dd")).toEqual(null);
});