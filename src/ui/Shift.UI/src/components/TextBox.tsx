import { ChangeEvent, FocusEvent, ForwardedRef, HTMLInputTypeAttribute } from "react";
import { FieldError } from "react-hook-form";
import { errorHelper } from "@/helpers/errorHelper";

interface Props {
    ref?: ForwardedRef<HTMLInputElement>,
    name?: string;
    type?: HTMLInputTypeAttribute | undefined;
    autoFocus?: boolean;
    className?: string;
    maxLength?: number;
    placeholder?: string;
    disabled?: boolean;
    readOnly?: boolean;
    error?: FieldError;
    onBlur?: (e: FocusEvent<HTMLInputElement>) => void;
    onChange?: (e: ChangeEvent<HTMLInputElement>) => void;
}

export default function TextBox({
    ref,
    name,
    type,
    autoFocus,
    className,
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
        <input
            ref={ref}
            autoFocus={autoFocus}
            name={name}
            className={`form-control insite-text ${error ? "is-invalid" : ""} ${className ?? ""}`}
            maxLength={maxLength}
            placeholder={placeholder}
            disabled={disabled}
            readOnly={readOnly}
            type={type}
            title={errorTooltip ?? undefined}
            onBlur={onBlur}
            onChange={onChange}
        />
    );
};