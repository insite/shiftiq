import { errorHelper } from "@/helpers/errorHelper";
import { ReactNode } from "react";
import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";
import ActionLink from "../ActionLink";

interface Props<T extends object> {
    children?: ReactNode;
    className?: string;
    label?: string;
    description?: string;
    editHref?: string | null;
    editTitle?: string | null;
    hasBottomMargin?: boolean;
    required?: boolean;
    error?: FieldError | Merge<FieldError, FieldErrorsImpl<T>> | null;
}

export default function FormField<T extends object>({
    children,
    className,
    label,
    description,
    editHref,
    editTitle,
    hasBottomMargin,
    required,
    error
}: Props<T>) {
    let fieldClass = "form-group";
    if (className) {
        fieldClass += " " + className;
    }
    if (hasBottomMargin === undefined || hasBottomMargin) {
        fieldClass += " mb-3";
    }

    const errorTooltip = errorHelper.getErrorTooltip(error);

    return (
        <div className={fieldClass}>
            {editHref && (
                <div className="float-end">
                    <ActionLink href={editHref}>
                        <i className="fas icon fa-pencil" title={editTitle ?? ""}></i>
                    </ActionLink>
                </div>
            )}
            {label && (
                <label className="form-label" title={errorTooltip ?? undefined}>
                    {label}
                    {(required || error) && (
                        <sup className="text-danger ms-1">
                            <i className="far fa-asterisk fa-xs"></i>
                        </sup>
                    )}
                    {error && (
                        <sup className="text-danger">
                            <i className="far fa-exclamation ms-1"></i>
                        </sup>
                    )}
                </label>
            )}
            <div>
                {children}
            </div>
            {description && (
                <div className="form-text">
                    {description}
                </div>
            )}
        </div>
    );
}
