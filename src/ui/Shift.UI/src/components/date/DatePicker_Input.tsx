import { ChangeEvent, KeyboardEvent, useRef } from "react";

interface Props {
    defaultValue: number;
    min: number;
    max: number;
    showLeadingZero: boolean;
    onChange: (value: number) => void;
    onClose: () => void;
}

export default function DatePicker_Input({ defaultValue, min, max, showLeadingZero, onChange, onClose }: Props) {
    const inputRef = useRef<HTMLInputElement>(null);

    function handleKeyDown(e: KeyboardEvent<HTMLInputElement>) {
        const isAllowed =
            e.ctrlKey
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
            ;

        if (!isAllowed) {
            e.preventDefault();
        }

        if (e.key === "Enter") {
            onClose();            
        }
    }

    function handleChange(e: ChangeEvent<HTMLInputElement>) {
        const value = Number(e.target.value);
        if (!Number.isNaN(value) && value >= min && value <= max) {
            onChange(value);
        }
    }

    function handleBlur() {
        if (!inputRef.current) {
            return;
        }
        inputRef.current.value = !showLeadingZero || defaultValue > 9 ? String(defaultValue) : "0" + String(defaultValue);
    }

    return (
        <input
            ref={inputRef}
            defaultValue={!showLeadingZero || defaultValue > 9 ? String(defaultValue) : "0" + String(defaultValue)}
            maxLength={2}
            onKeyDown={handleKeyDown}
            onChange={handleChange}
            onBlur={handleBlur}
        />
    );
}