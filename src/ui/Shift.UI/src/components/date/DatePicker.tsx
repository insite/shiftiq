import { ForwardedRef, useImperativeHandle, useRef } from "react";
import { FieldError } from "react-hook-form";
import { errorHelper } from "@/helpers/errorHelper";
import { DateOrDateTime } from "@/helpers/date/dateTimeTypes";
import DatePicker_Calendar from "./DatePicker_Calendar";
import { translate } from "@/helpers/translate";
import { TimeZoneId } from "@/helpers/date/timeZones";
import DatePicker_TimeZone from "./DatePicker_TimeZone";
import { useDatePicker } from "./useDatePicker";

import "./DatePicker.css";

export interface DatePickerProps {
    ref?: ForwardedRef<HTMLInputElement>,
    name?: string;
    showTime?: boolean;
    defaultTimeZoneId?: TimeZoneId;
    className?: string;
    disabled?: boolean;
    readOnly?: boolean;
    value?: DateOrDateTime;
    defaultValue?: DateOrDateTime;
    placeholder?: string;
    error?: FieldError;
    onChange?: (value: DateOrDateTime) => void;
    onBlur?: () => void;
}

export default function DatePicker({
    ref,
    name,
    showTime,
    defaultTimeZoneId,
    className,
    disabled,
    readOnly,
    value,
    defaultValue,
    placeholder,
    error,
    onChange,
    onBlur,
}: DatePickerProps) {
    const {
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
    } = useDatePicker(
        showTime,
        defaultTimeZoneId,
        disabled,
        readOnly,
        value,
        defaultValue,
        handleFocusInput,
        onChange,
        onBlur
    );

    const inputRef = useRef<HTMLInputElement>(null);
    useImperativeHandle<HTMLInputElement | null, HTMLInputElement | null>(ref, () => inputRef.current);

    const timeZoneButtonRef = useRef<HTMLButtonElement>(null);
    const calendarButtonRef = useRef<HTMLButtonElement>(null);

    const errorTooltip = errorHelper.getErrorTooltip(error) ?? "";

    function handleFocusInput() {
        inputRef.current?.focus();
    }

    return (
        <div className={"datepicker " + (className ?? "")}>
            <input
                ref={inputRef}
                name={name}
                className={`form-control ${errorTooltip ? "is-invalid" : ""}`}
                title={errorTooltip}
                disabled={disabled}
                readOnly={readOnly}
                value={text}
                placeholder={placeholder}
                maxLength={50}
                data-showtime={showTime ? true : undefined}
                onKeyDown={handleKeyDown}
                onChange={e => setText(e.target.value)}
                onBlur={handleBlur}
            />
            {showTime && (
                <button
                    ref={timeZoneButtonRef}
                    type="button"
                    className="open-timezone"
                    disabled={disabled || readOnly}
                    tabIndex={-1}
                    title={translate("Select Time Zone")}
                    onClick={handleTimeZoneButtonClicked}
                >
                    <i className="far fa-globe-americas"></i>
                    <span>{timeZone.timeZoneAbbrev}</span>
                </button>
            )}
            <button
                ref={calendarButtonRef}
                type="button"
                className="open-calendar"
                disabled={disabled || readOnly}
                tabIndex={-1}
                title={showTime ? translate("Select Date/Time") : translate("Select Date")}
                onClick={handleCalendarButtonClicked}
            >
                <i className="far fa-calendar-alt"></i>
            </button>
            {calendarState.opened && inputRef.current && (
                <DatePicker_Calendar
                    relative={inputRef.current}
                    trigger={calendarButtonRef.current}
                    value={calendarState.dateTime}
                    showTime={showTime}
                    onChange={handleCalendarChange}
                    onClose={handleCalendarClose}
                />
            )}
            {isTimeZoneOpen && inputRef.current && (
                <DatePicker_TimeZone
                    relative={inputRef.current}
                    trigger={timeZoneButtonRef.current}
                    selectedTimeZoneId={timeZone.timeZoneId}
                    onSelect={handleTimeZoneSelect}
                    onClose={handleTimeZoneClose}
                />
            )}
        </div>
    );
}