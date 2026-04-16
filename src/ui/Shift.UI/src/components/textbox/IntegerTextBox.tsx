import { ForwardedRef, HTMLInputTypeAttribute, useEffect, useState } from "react";
import { FieldError } from "react-hook-form";
import { errorHelper } from "@/helpers/errorHelper";
import { numberHelper } from "@/helpers/numberHelper";

export interface IntegerTextBoxProps {
    ref?: ForwardedRef<HTMLInputElement>,
    name?: string;
    type?: HTMLInputTypeAttribute | undefined;
    autoFocus?: boolean;
    className?: string;
    maxLength?: number;
    placeholder?: string;
    disabled?: boolean;
    readOnly?: boolean;
    value?: number | null;
    defaultValue?: number | null;
    error?: FieldError | string;
    onBlur?(): void;
    onChange?(value: number | null): void;
    onKeyDown?: React.KeyboardEventHandler<HTMLInputElement>;
}

export default function IntegerTextBox({
    ref,
    name,
    type,
    autoFocus,
    className,
    maxLength,
    placeholder,
    disabled,
    readOnly,
    value,
    defaultValue,
    error,
    onBlur,
    onChange,
    onKeyDown
}: IntegerTextBoxProps) {
    const [storedValue, setStoredValue] = useState(defaultValue ?? null);

    const currentValue = value !== undefined ? value : storedValue;
    const setCurrentValue = value !== undefined
        ? (onChange ?? (() => {})) : (
            (newValue: number | null) => {
                setStoredValue(newValue);
                onChange?.(newValue);
            }
        );

    const [text, setText] = useState(() => format(currentValue));

    const errorTooltip = typeof error === "string" ? error : errorHelper.getErrorTooltip(error);

    useEffect(() => {
        setText(format(currentValue));
    }, [currentValue]);

    function handleKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
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
            || e.key === ","
            ;

        if (e.key === "Enter") {
            submitChangedValue();
        }

        if (!isAllowed) {
            e.preventDefault();
        } else {
            onKeyDown?.(e);
        }
    }

    function handleBlur() {
        submitChangedValue();
        onBlur?.();
    }

    function submitChangedValue() {
        const newValue = text
            ? numberHelper.parseInt(text)
            : null;

        setCurrentValue(newValue);
    }

    return (
        <input
            ref={ref}
            autoFocus={autoFocus}
            name={name}
            className={`form-control insite-text ${error ? "is-invalid" : ""} ${className ?? ""}`}
            maxLength={maxLength}
            placeholder={placeholder}
            value={text}
            disabled={disabled}
            readOnly={readOnly}
            type={type}
            title={errorTooltip ?? undefined}
            onBlur={handleBlur}
            onChange={e => setText(e.target.value)}
            onKeyDown={handleKeyDown}
        />
    );
};

function format(value: number | null | undefined) {
    return value !== null && value !== undefined ? String(value) : "";
}