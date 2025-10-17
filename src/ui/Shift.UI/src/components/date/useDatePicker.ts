import { useState, KeyboardEvent, useEffect } from "react";
import { DateFormatType, DateOrDateTime, DateTimeParts, isDateTime, isDateTimeInvalid } from "@/helpers/date/dateTimeTypes";
import { TimeZoneId, timeZones } from "@/helpers/date/timeZones";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";

export const dateInputFormatType: DateFormatType = "mmm d, yyyy";

interface TimeZone {
    timeZoneId: TimeZoneId;
    timeZoneAbbrev: string;
}

export function useDatePicker(
    showTime: boolean | undefined,
    defaultTimeZoneId: TimeZoneId | undefined,
    disabled: boolean | undefined,
    readOnly: boolean | undefined,
    value: DateOrDateTime | undefined,
    defaultValue: DateOrDateTime | undefined,
    onFocusInput: () => void,
    onChange: ((value: DateOrDateTime) => void) | undefined,
    onBlur: (() => void) | undefined
) {
    const [storedValue, setStoredValue] = useState(value !== undefined ? value : defaultValue ?? null);

    const currentValue = value !== undefined ? value : storedValue;
    const setCurrentValue = value !== undefined
        ? (onChange ?? (() => {}))
        : (newValue: DateOrDateTime) => {
            setStoredValue(newValue);
            onChange?.(newValue);
        };

    const [text, setText] = useState(() => convertToText(showTime, currentValue));

    const [timeZone, setTimeZone] = useState<TimeZone>(() => {
        if (!showTime) {
            return { timeZoneId: "UTC", timeZoneAbbrev: "UTC" };
        }
        if (currentValue && !("time" in currentValue)) {
            throw new Error(`Invalid datetime value type: ${JSON.stringify(currentValue)}`);
        }

        const timeZoneId: TimeZoneId = currentValue?.time?.timeZoneId ?? defaultTimeZoneId ?? "UTC";
        const timeZone = timeZones.find(x => x.timeZoneId === timeZoneId);

        return timeZone ? {
            timeZoneId,
            timeZoneAbbrev: timeZone.abbrev,
        }
        : {
            timeZoneId: "UTC",
            timeZoneAbbrev: "UTC",
        }
    });

    const [calendarState, setCalendarState] = useState<{ opened: boolean, dateTime: DateTimeParts | null }>({ opened: false, dateTime: null });
    const [isTimeZoneOpen, setIsTimeZoneOpen] = useState(false);

    useEffect(() => {
        const newText = convertToText(showTime, currentValue);
        setText(newText);
    }, [showTime, currentValue]);

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        if (disabled || readOnly) {
            return;
        }

        const isAllowed =
            e.ctrlKey
            || (e.shiftKey && e.key === "Insert")
            || e.key === "Tab"
            || e.key >= "0" && e.key <= "9"
            || e.key === "Control"
            || e.key === "Shift"
            || e.key === "ArrowLeft"
            || e.key === "ArrowRight"
            || e.key === "End"
            || e.key === "Home"
            || e.key === "Clear"
            || e.key === "Delete"
            || e.key === "Backspace"
            || e.key === "Enter"
            || e.key >= "a" && e.key <= "z"
            || e.key >= "A" && e.key <= "Z"
            || dateTimeHelper.isDateSeparator(e.key)
            || showTime && dateTimeHelper.isTimeSeparator(e.key)
            ;

        if (e.key === "Enter") {
            submitChangedValue();
        }

        if (!isAllowed) {
            e.preventDefault();
        }
    }

    function handleBlur() {
        submitChangedValue();
        onBlur?.();
    }

    function handleCalendarButtonClicked() {
        if (calendarState.opened) {
            setCalendarState({
                opened: false,
                dateTime: null
            });
            onFocusInput();
            return;
        }

        let dateTime: DateTimeParts | null;

        if (!currentValue || isDateTimeInvalid(currentValue)) {
            dateTime = null;
        } else if (isDateTime(currentValue)) {
            dateTime = currentValue;
        } else {
            dateTime = {
                time: { hour: 0, minute: 0, timeZoneId: timeZone.timeZoneId },
                date: currentValue
            };
        }

        setCalendarState({
            opened: true,
            dateTime
        });
    }

    function handleCalendarChange(value: DateTimeParts) {
        value.time.timeZoneId = timeZone.timeZoneId;

        if (!showTime) {
            onFocusInput();
        }

        setCalendarState({
            opened: !!showTime,
            dateTime: value
        });

        setCurrentValue(showTime ? value : value.date);
    }

    function handleTimeZoneButtonClicked() {
        if (isTimeZoneOpen) {
            onFocusInput();
        }
        setIsTimeZoneOpen(!isTimeZoneOpen);
    }

    function handleCalendarClose(escPressed: boolean) {
        if (escPressed) {
            onFocusInput();
        }

        setCalendarState({
            opened: false,
            dateTime: null
        });
    }

    function handleTimeZoneSelect(timeZoneId: TimeZoneId) {
        syncTimeZone(timeZoneId);
        submitChangedValue(timeZoneId);
        handleTimeZoneClose(true);
    }

    function handleTimeZoneClose(escPressed: boolean) {
        if (escPressed) {
            onFocusInput();
        }
        setIsTimeZoneOpen(false);
    }

    function submitChangedValue(forceTimeZoneId?: TimeZoneId) {
        let newValue: DateOrDateTime;

        if (!text) {
            newValue = null;
        }
        else if (showTime) {
            newValue = dateTimeHelper.parseDateTime(text, forceTimeZoneId ?? timeZone.timeZoneId);
            if (newValue) {
                if (forceTimeZoneId) {
                    newValue.time.timeZoneId = forceTimeZoneId;
                }
                syncTimeZone(newValue.time.timeZoneId!);
            } else {
                newValue = {
                    isInvalid: true,
                    text
                };
            }
        } else {
            newValue = dateTimeHelper.parseDate(text);
            if (!newValue) {
                newValue = {
                    isInvalid: true,
                    text
                };
            }
        }

        setCurrentValue(newValue);
    }

    function syncTimeZone(timeZoneId: TimeZoneId) {
        if (timeZoneId === timeZone.timeZoneId) {
            return;
        }

        const timeZoneAbbrev = timeZones.find(x => x.timeZoneId === timeZoneId)?.abbrev ?? "UTC";

        setTimeZone({
            timeZoneId,
            timeZoneAbbrev
        });
    }

    return {
        text,
        timeZone,
        calendarState,
        isTimeZoneOpen,
        setText,
        handleKeyDown,
        handleBlur,
        handleCalendarButtonClicked,
        handleCalendarChange,
        handleTimeZoneButtonClicked,
        handleCalendarClose,
        handleTimeZoneSelect,
        handleTimeZoneClose
    };
}

function convertToText(showTime: boolean | undefined, value: DateOrDateTime | undefined): string {
    if (!value) {
        return "";
    }

    if ("isInvalid" in value) {
        return value.text;
    }

    if (showTime) {
        if ("time" in value) {
            return dateTimeHelper.formatDateTime(value, dateInputFormatType)!;    
        }
        throw new Error(`Invalid datetime value type: ${JSON.stringify(value)}`);
    }

    if ("year" in value) {
        return value ? dateTimeHelper.formatDate(value, dateInputFormatType)! : "";
    }

    throw new Error(`Invalid date value type: ${JSON.stringify(value)}`);
}