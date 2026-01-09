import { ChangeEvent, FocusEvent, ForwardedRef } from "react";
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
    error?: FieldError;
    onBlur?: (e: FocusEvent<HTMLTextAreaElement>) => void;
    onChange?: (e: ChangeEvent<HTMLTextAreaElement>) => void;
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
    error,
    onBlur,
    onChange
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
            disabled={disabled}
            readOnly={readOnly}
            title={errorTooltip ?? undefined}
            onBlur={onBlur}
            onChange={onChange}
        />
    );
};