import { _dateTimeParser } from "./_dateTimeParser";
import { DateParts, DateFormatType, months, TimeParts, DateTimeParts, isDateTimeInvalid, DateTime } from "./dateTimeTypes";
import { TimeZoneId, timeZones } from "./timeZones";

const _dateFormatters = {
    "mm/dd/yyyy": (date: DateParts) => {
        return isValidDate(date) ? `${padZero(date.month!)}/${padZero(date.day!)}/${date.year!}` : null;
    },
    "yyyy-mm-dd": (date: DateParts) => {
        return isValidDate(date) ? `${date.year}-${padZero(date.month!)}-${padZero(date.day!)}` : null;
    },
    "mmm d, yyyy": (date: DateParts) => {
        return isValidDate(date) ? `${months[date.month! - 1].short} ${date.day!}, ${date.year}` : null;
    },
}

const _timeFormatters = {
    "h:m tt z": (dateTime: DateTimeParts) => {
        const { date, time } = dateTime;

        if (!isValidTime(time)) {
            return null;
        }

        const { hour, minute, timeZoneId } = time;

        const formatter = new Intl.DateTimeFormat("en-CA", {
            timeZone: timeZoneId!,
            timeZoneName: "short",
        });
        const parts = formatter.formatToParts(new Date(date.year!, date.month! - 1, date.day!, hour!, minute!));
        const timeZonePart = parts.find(x => x.type === 'timeZoneName')?.value ?? "Unknown";

        const hourAdj = hour! > 12
            ? hour! - 12
            : hour === 0
                ? 12
                : hour!;

        const amPm = hour! < 12 ? "AM" : "PM";

        return `${hourAdj}:${padZero(minute)} ${amPm} ${timeZonePart}`;
    },
}

function isValidDate({ day, month, year }: DateParts) {
    return year && year >= _dateTimeParser.minYear && year <= _dateTimeParser.maxYear &&
        month && month >= 1 && month <= 12 &&
        day && day >= 1 && day <= _dateTimeParser.getDaysInMonth(year, month);
}

function isValidTime({ hour, minute, timeZoneId }: TimeParts) {
    return hour !== null && hour >= 0 && hour <= 23
        && minute !== null && minute >= 0 && minute <= 59
        && timeZones.find(x => x.timeZoneId === timeZoneId);
}

function padZero(n: number | null) {
    return n !== null && n < 10 ? "0" + String(n) : String(n);
}

function formatTimeOffset(dateTime: DateTimeParts): string {
    const formatter = new Intl.DateTimeFormat("en-CA", {
        timeZone: dateTime.time.timeZoneId ?? "UTC",
        timeZoneName: "shortOffset"
    });
    
    const parts = formatter.formatToParts(new Date());
    const timeZoneOffset = parts.find(x => x.type === "timeZoneName")!.value.substring(3);

    if (!timeZoneOffset) {
        return "+00:00";
    }
    
    const sign = timeZoneOffset[0];
    const [hour, minute] = timeZoneOffset.substring(1).split(":");

    const formattedHour = hour.length === 1 ? "0" + hour : hour;
    const formattedMinute = !minute ? "00" : minute.length === 1 ? "0" + minute : minute;

    return `${sign}${formattedHour}:${formattedMinute}`;
}

export const dateTimeHelper = {
    parseDate: _dateTimeParser.parseDate,
    parseDateTime: _dateTimeParser.parseDateTime,

    parseServerDateTime(dateTime: string | undefined | null, timeZoneId: TimeZoneId): DateTimeParts | null {
        if (!dateTime) {
            return null;
        }
        try {
            return changeTimeZone(new Date(dateTime), timeZoneId);
        } catch {
            throw new Error(`"${dateTime}" is not a valid datetime.`);
        }
    },
    
    isDateSeparator(c: string) {
        return _dateTimeParser.dateSeparators.includes(c);
    },

    isTimeSeparator(c: string) {
        return _dateTimeParser.timeSeparator === c;
    },

    getDaysInMonth: _dateTimeParser.getDaysInMonth,

    formatDate(date: DateParts | undefined | null, formatType: DateFormatType): string | null {
        return date
            ? _dateFormatters[formatType](date)
            : null;
    },

    formatDateTime(dateTime: DateTimeParts | undefined | null, dateFormatType: DateFormatType, separator: string = " "): string | null {
        if (!dateTime) {
            return null;
        }

        const dateFormatted = _dateFormatters[dateFormatType](dateTime.date);
        if (!dateFormatted) {
            return null;
        }

        const timeFormatted = _timeFormatters["h:m tt z"](dateTime);
        if (!timeFormatted) {
            return null;
        }

        return `${dateFormatted}${separator}${timeFormatted}`;
    },

    formatServerDateTime(dateTime: DateTime | undefined): string | null {
        if (!dateTime || isDateTimeInvalid(dateTime)) {
            return null;
        }

        const timeZoneOffset = formatTimeOffset(dateTime);
        const { year, month, day } = dateTime.date;
        const { hour, minute } = dateTime.time;

        return `${year}-${padZero(month)}-${padZero(day)}T${padZero(hour)}:${padZero(minute)}:00${timeZoneOffset}`;
    },

    formatTime(dateTime: DateTimeParts | undefined | null): string | null {
        if (!dateTime) {
            return null;
        }
        return _timeFormatters["h:m tt z"](dateTime);
    },

    today(): DateParts {
        const n = new Date();
        return {
            day: n.getDate(),
            month: n.getMonth() + 1,
            year: n.getFullYear(),
        }
    },

    now(timeZoneId: TimeZoneId = "UTC"): DateTimeParts {
        return changeTimeZone(new Date(), timeZoneId);
    },
}

function changeTimeZone(dateTime: Date, destTimeZoneId: TimeZoneId): DateTimeParts;
function changeTimeZone(dateTime: null | undefined, destTimeZoneId: TimeZoneId): null;
function changeTimeZone(dateTime: Date | null | undefined, destTimeZoneId: TimeZoneId): DateTimeParts | null {
    if (!dateTime) {
        return null;
    }

    const formatter = new Intl.DateTimeFormat("en-CA", {
        timeZone: destTimeZoneId,
        day: "numeric",
        month: "numeric",
        year: "numeric",
        hour: "numeric",
        minute: "numeric",
        hour12: false,
    });
    
    const parts = formatter.formatToParts(dateTime);

    const day = Number(parts.find(x => x.type === "day")!.value);
    const month = Number(parts.find(x => x.type === "month")!.value);
    const year = Number(parts.find(x => x.type === "year")!.value);
    const hour = Number(parts.find(x => x.type === "hour")!.value);
    const minute = Number(parts.find(x => x.type === "minute")!.value);

    return {
        date: {
            day,
            month,
            year,
        },
        time: {
            hour,
            minute,
            timeZoneId: destTimeZoneId,
        }
    }
}
