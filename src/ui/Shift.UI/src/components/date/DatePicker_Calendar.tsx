import { useMemo, useState } from "react";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { DateTimeParts, months } from "@/helpers/date/dateTimeTypes";
import { translate } from "@/helpers/translate";
import ChevronLeft from "../svg/ChevronLeft";
import ChevronRight from "../svg/ChevronRight";
import "./DatePicker_Calendar.css";
import DatePicker_Input from "./DatePicker_Input";
import PopupPanel from "../PopupPanel";

const _weekdays = [
    translate("Sun"),
    translate("Mon"),
    translate("Tue"),
    translate("Wed"),
    translate("Tue"),
    translate("Fri"),
    translate("Sat"),
];

interface Day {
    key: number;
    year: number;
    month: number;
    day: number;
}

interface Props {
    relative: HTMLElement,
    trigger: HTMLButtonElement | null;
    value: DateTimeParts | null;
    showTime?: boolean,
    onChange: (value: DateTimeParts) => void;
    onClose: (escPressed: boolean) => void;
}

export default function DateInput_Calendar({ relative, trigger, value, showTime, onChange, onClose }: Props) {
    const now = new Date();

    const [{ currentYear, currentMonth }, setCurrentMonth] = useState(() => ({
        currentYear: value?.date?.year ?? now.getFullYear(),
        currentMonth: getMonth(value?.date?.month ?? (now.getMonth() + 1))
    }));

    const days = useMemo(() => getDays(currentYear, currentMonth.number), [currentYear, currentMonth]);

    function selectDay(newYear: number, newMonth: number, newDay: number) {
        if (newMonth != currentMonth.number) {
            setCurrentMonth({
                currentYear: newYear,
                currentMonth: getMonth(newMonth)
            });
        }
        onChange({
            time: {
                hour: value?.time?.hour ?? 0,
                minute: value?.time?.minute ?? 0,
                timeZoneId: value?.time?.timeZoneId ?? "UTC"
            },
            date: {
                year: newYear,
                month: newMonth,
                day: newDay
            }
        });
    }

    function selectNextMonth() {
        const newMonth = currentMonth.number < 12 ? currentMonth.number + 1 : 1;
        const newYear = currentMonth.number < 12 ? currentYear : currentYear + 1;

        setCurrentMonth({
            currentYear: newYear,
            currentMonth: getMonth(newMonth)
        });
    }

    function selectPrevMonth() {
        const newMonth = currentMonth.number > 1 ? currentMonth.number - 1 : 12;
        const newYear = currentMonth.number > 1 ? currentYear : currentYear - 1;

        setCurrentMonth({
            currentYear: newYear,
            currentMonth: getMonth(newMonth)
        });
    }

    function handelChange(hour: number | null, minute: number | null, changeAmPm: boolean) {
        if (hour !== null) {
            if (hour === 12) {
                hour = 0;
            }        
            if (value?.time && value.time.hour! >= 12) {
                hour += 12;
            }
        }

        let newHour = hour ?? value?.time?.hour ?? 0;
        const newMinute = minute ?? value?.time?.minute ?? 0;

        if (changeAmPm) {
            if (newHour < 12) {
                newHour += 12;
            } else {
                newHour -= 12;
            }
        }

        onChange({
            time: {
                hour: newHour,
                minute: newMinute,
                timeZoneId: value?.time?.timeZoneId ?? "UTC"
            },
            date: {
                year: value?.date?.year ?? now.getFullYear(),
                month: value?.date?.month ?? now.getMonth() + 1,
                day: value?.date?.day ?? now.getDate()
            }
        });
    }
    
    return (
        <PopupPanel
            relative={relative}
            trigger={trigger}
            className="datepicker-calendar"
            onClose={onClose}
        >
            <header>
                <div className="year-and-month">
                    {currentMonth.name} {currentYear}
                </div>
                <div className="navigation">
                    <button
                        type="button"
                        tabIndex={-1}
                        className="me-2"
                        title={translate("Previous Month")}
                        onClick={selectPrevMonth}
                    >
                        <ChevronLeft />
                    </button>
                    <button
                        type="button"
                        tabIndex={-1}
                        title={translate("Next Month")}
                        onClick={selectNextMonth}
                    >
                        <ChevronRight />
                    </button>
                </div>
            </header>
            <div className="days">
                {_weekdays.map((d, index) => (
                    <div key={index}>
                        {d}
                    </div>
                ))}
                {days.map(d => (
                    <button
                        key={d.key}
                        type="button"
                        tabIndex={-1}
                        data-other-month={d.month !== currentMonth.number ? true : undefined}
                        data-now={d.year === now.getFullYear() && d.month === now.getMonth() + 1 && d.day === now.getDate() ? true : undefined}
                        data-selected={value && d.year === value.date.year && d.month === value.date.month && d.day === value.date.day ? true : undefined}
                        onClick={() => selectDay(d.year, d.month, d.day)}
                    >
                        {d.day}
                    </button>
                ))}
            </div>
            {showTime && (
                <div className="time-panel">
                    <DatePicker_Input
                        defaultValue={value ? (value.time.hour! > 12 ? value.time.hour! - 12 : (value.time.hour === 0 ? 12 : value.time.hour!)): 12}
                        min={1}
                        max={12}
                        showLeadingZero={false}
                        onChange={value => handelChange(value, null, false)}
                        onClose={() => onClose(true)}
                    />
                    <span>
                        :
                    </span>
                    <DatePicker_Input
                        defaultValue={value?.time?.minute ?? 0}
                        min={0}
                        max={59}
                        showLeadingZero={true}
                        onChange={value => handelChange(null, value, false)}
                        onClose={() => onClose(true)}
                    />
                    <button
                        type="button"
                        onClick={() => handelChange(null, null, true)}
                    >
                        {value && value.time.hour! >= 12 ? "PM" : "AM"}
                    </button>
                </div>
            )}
        </PopupPanel>
    );
}

function getDays(year: number, month: number): Day[] {
    const days: Day[] = [];

    const daysInMonth = dateTimeHelper.getDaysInMonth(year, month);
    const firstDayOfWeek = new Date(year, month - 1, 1).getDay();
    const lastDayOfWeek = new Date(year, month - 1, daysInMonth).getDay();

    if (firstDayOfWeek !== 0) {
        const prevYear = month > 1 ? year : year - 1;
        const prevMonth = month > 1 ? month - 1 : 12;
        const prevDaysInMonth = dateTimeHelper.getDaysInMonth(prevYear, prevMonth);

        for (let i = 0; i < firstDayOfWeek; i++) {
            const prevDay = prevDaysInMonth - firstDayOfWeek + i + 1;
            days.push({ key: prevMonth * 100 + prevDay, year: prevYear, month: prevMonth, day: prevDay});
        }
    }

    for (let i = 1; i <= daysInMonth; i++) {
        days.push({ key: month * 100 + i, year, month, day: i});
    } 

    if (lastDayOfWeek !== 6) {
        const nextYear = month < 12 ? year : year + 1;
        const nextMonth = month < 12 ? month + 1 : 1;

        for (let i = lastDayOfWeek; i < 6; i++) {
            const nextDay = i - lastDayOfWeek + 1;
            days.push({ key: nextMonth * 100 + nextDay, year: nextYear, month: nextMonth, day: nextDay});
        }
    }

    return days;
}

function getMonth(month: number) {
    if (month < 1 || month > 12) {
        throw new Error(`Invalid month: ${month}`);
    }
    return months[month - 1];
}