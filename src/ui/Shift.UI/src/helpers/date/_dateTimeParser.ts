import { DateParts, DateFormatType, months, TimeParts, TimeFormatType } from "./dateTimeTypes";
import { TimeZoneId, timeZones } from "./timeZones";

interface SplitText {
    date: string[];
    time: string[];
}

const _dateSeparators = ["/", "\\", "-", " ", ","];
const _timeSeparator = ":";
const _minYear = 1900;
const _maxYear = new Date().getFullYear() + 100;

const _dateParsers = {
    "mm/dd/yyyy": (parts: string[]): DateParts | null => {
        if (parts.length < 2 || parts.length > 3) {
            return null;
        }

        const year = parts.length === 3 ? Number(parts[2]) : new Date().getFullYear();
        if (Number.isNaN(year) || year < _minYear || year > _maxYear) {
            return null;
        }

        const month = Number(parts[0]);
        if (Number.isNaN(month) || month < 1 || month > 12) {
            return null;
        }

        const day = Number(parts[1]);
        if (Number.isNaN(day) || day < 1 || day > getDaysInMonth(year, month)) {
            return null;
        }

        return {
            day,
            month,
            year
        };
    },
    "yyyy-mm-dd": (parts: string[]): DateParts | null => {
        if (parts.length !== 3) {
            return null;
        }

        const year = Number(parts[0]);
        if (Number.isNaN(year) || year < _minYear || year > _maxYear) {
            return null;
        }

        const month = Number(parts[1]);
        if (Number.isNaN(month) || month < 1 || month > 12) {
            return null;
        }

        const day = Number(parts[2]);
        if (Number.isNaN(day) || day < 1 || day > getDaysInMonth(year, month)) {
            return null;
        }

        return {
            day,
            month,
            year
        };
    },
    "mmm d, yyyy": (parts: string[]): DateParts | null => {
        if (parts.length < 2 || parts.length > 3) {
            return null;
        }

        const year = parts.length === 3 ? Number(parts[2]) : new Date().getFullYear();
        if (Number.isNaN(year) || year < _minYear || year > _maxYear) {
            return null;
        }

        const month = getMonthByName(parts[0])?.number;
        if (!month || month < 1 || month > 12) {
            return null;
        }

        const day = Number(parts[1]);
        if (Number.isNaN(day) || day < 1 || day > getDaysInMonth(year, month)) {
            return null;
        }

        return {
            day,
            month,
            year
        };
    },
}

const _timeParsers = {
    "h:m tt z": (parts: string[]): TimeParts | null => {
        if (parts.length < 2 || parts.length > 4) {
            return null;
        }

        let hour = Number(parts[0]);
        if (Number.isNaN(hour) || hour < 1 || hour > 12) {
            return null;
        }

        const minute = Number(parts[1]);
        if (Number.isNaN(minute) || minute < 0 || minute > 59) {
            return null;
        }

        const amPm = parts.length >= 3 ? parts[2].toLowerCase() : null;
        switch (amPm) {
            case "pm":
                if (hour < 12) {
                    hour += 12;
                }
                break;
            case "am":
            default:
                if (hour === 12) {
                    hour = 0;
                }
                break;            
        }

        let timeZoneId: TimeZoneId | null;
        if (parts.length >= 4) {
            const timeZoneCode = parts[3].toLowerCase();
            timeZoneId = timeZones.find(x =>
                x.abbrev.toLowerCase() === timeZoneCode
                || x.standardCode?.toLowerCase() === timeZoneCode
                || x.daylightCode?.toLowerCase() === timeZoneCode
            )?.timeZoneId ?? null;
            if (!timeZoneId) {
                return null;
            }
        } else {
            timeZoneId = null;
        }

        return {
            hour,
            minute,
            timeZoneId
        };
    },
}

function getMonthByName(monthName: string) {
    monthName = monthName.toLowerCase();
    return months.find(x => x.name.toLowerCase() === monthName || x.short.toLowerCase() === monthName);
}

function splitText(s: string | undefined | null): SplitText | null {
    if (!s) {
        return null;
    }

    const result: SplitText = {
        date: [],
        time: [],
    };

    let current = "";
    let isTime = false;

    for (let i = 0; i < s.length; i++) {
        if (!_dateSeparators.includes(s[i]) && s[i] !== _timeSeparator) {
            current += s[i];
        } else  if (current) {
            if (!isTime && s[i] === _timeSeparator) {
                isTime = true;
            }
            if (isTime) {
                result.time.push(current);
            } else {
                result.date.push(current);
            }
            current = "";
        }
    }

    if (current) {
        if (isTime) {
            result.time.push(current);
        } else {
            result.date.push(current);
        }
    }

    return result.date.length || result.time.length ? result : null;
}

function getDaysInMonth(year: number, month: number) {
    return new Date(year, month, 0).getDate();
}

function parseDate(parts: string[], formatType: DateFormatType | DateFormatType[]) {
    const list = Array.isArray(formatType) ? formatType : [formatType];
    for (const current of list) {
        const result = _dateParsers[current](parts);
        if (result) {
            return result;
        }
    }

    return null;
}

function parseTime(
    parts: string[],
    defaultTimeZoneId: TimeZoneId,
    formatType: TimeFormatType | TimeFormatType[],
): TimeParts | null {
    if (!parts.length) {
        return {
            hour: 0,
            minute: 0,
            timeZoneId: defaultTimeZoneId,
        }
    }

    let time: TimeParts | null = null;

    const list = Array.isArray(formatType) ? formatType : [formatType];
    for (const current of list) {
        const result = _timeParsers[current](parts);
        if (result) {
            time = result;
            break;
        }
    }

    if (!time) {
        return null;
    }

    if (!time.timeZoneId) {
        time.timeZoneId = defaultTimeZoneId;
    }

    return time;
}

export const _dateTimeParser = {
    dateParsers: _dateParsers,
    timeParsers: _timeParsers,
    dateSeparators: _dateSeparators,
    timeSeparator: _timeSeparator,
    minYear: _minYear,
    maxYear: _maxYear,

    getDaysInMonth,
    splitText,

    parseDate(date: string | undefined | null, dateFormatType: DateFormatType | DateFormatType[] = ["mm/dd/yyyy", "mmm d, yyyy"]) {
        const parts = splitText(date);
        return parts && !parts.time.length ? parseDate(parts.date, dateFormatType) : null;
    },

    parseDateTime(
        dateTime: string | undefined | null,
        defaultTimeZoneId: TimeZoneId = "UTC",
        dateFormatType: DateFormatType | DateFormatType[] = ["mm/dd/yyyy", "mmm d, yyyy"],
        timeFormatType: TimeFormatType | TimeFormatType[] = ["h:m tt z"]

    ) {
        const parts = splitText(dateTime);
        if (!parts) {
            return null;
        }

        const date = parseDate(parts.date, dateFormatType);
        if (!date) {
            return null;
        }

        const time = parseTime(parts.time, defaultTimeZoneId, timeFormatType);
        if (!time) {
            return null;
        }

        return {
            date,
            time,
        }
    },
};