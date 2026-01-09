import { TimeZoneId, timeZones } from "@/helpers/date/timeZones";
import PopupPanel from "../PopupPanel";
import "./DatePicker_TimeZone.css";

interface Props {
    relative: HTMLElement,
    trigger: HTMLButtonElement | null;
    selectedTimeZoneId: TimeZoneId | null;
    onClose: (escPressed: boolean) => void;
    onSelect: (timeZoneId: TimeZoneId) => void;
}

export default function DatePicker_TimeZone({
    relative,
    trigger,
    selectedTimeZoneId,
    onClose,
    onSelect
}: Props) {
    return (
        <PopupPanel
            relative={relative}
            trigger={trigger}
            className="datepicker-timezone"
            onClose={onClose}
        >
            <div className="timezone-list">
                {timeZones.map(({ timeZoneId, title }) => (
                    <button
                        key={timeZoneId}
                        type="button"
                        tabIndex={-1}
                        data-selected={selectedTimeZoneId === timeZoneId ? true : undefined}
                        onClick={() => onSelect(timeZoneId)}
                    >
                        {title}
                    </button>
                ))}
            </div>
       </PopupPanel>
    );
}