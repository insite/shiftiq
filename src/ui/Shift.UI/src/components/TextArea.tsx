import { ForwardedRef } from "react";
import { FieldError } from "react-hook-form";
import { errorHelper } from "@/helpers/errorHelper";

interface Props {
    ref?: ForwardedRef<HTMLTextAreaElement>,
    name?: string;
    autoFocus?: boolean;
    className?: string;
    cols?: number;
    rows?: number;
    maxLength?: number;
    placeholder?: string;
    disabled?: boolean;
    readOnly?: boolean;
    value?: string;
    defaultValue?: string;
    error?: FieldError;
    onBlur?: (e: React.FocusEvent<HTMLTextAreaElement>) => void;
    onChange?: (e: React.ChangeEvent<HTMLTextAreaElement>) => void;
    onKeyDown?: React.KeyboardEventHandler<HTMLTextAreaElement>;
}

export default function TextArea({
    ref,
    name,
    autoFocus,
    className,
    cols,
    rows,
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
}: Props) {
    const errorTooltip = errorHelper.getErrorTooltip(error);

    return (
        <textarea
            ref={ref}
            autoFocus={autoFocus}
            name={name}
            className={`form-control insite-text ${error ? "is-invalid" : ""} ${className ?? ""}`}
            cols={cols}
            rows={rows}
            maxLength={maxLength}
            placeholder={placeholder}
            value={value}
            defaultValue={defaultValue}
            disabled={disabled}
            readOnly={readOnly}
            title={errorTooltip ?? undefined}
            onBlur={onBlur}
            onChange={onChange}
            onKeyDown={onKeyDown}
        />
    );
};