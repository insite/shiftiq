import "./FormField.css";

import { errorHelper } from "@/helpers/errorHelper";
import { ReactNode } from "react";
import { FieldError, FieldErrorsImpl, Merge } from "react-hook-form";
import ActionLink from "../ActionLink";
import Icon from "../icon/Icon";
import { IconName } from "../icon/IconName";
import { IconStyle } from "../icon/IconStyle";

interface Props<T extends object> {
    children?: ReactNode;
    className?: string;
    label?: string;
    description?: string;
    editHref?: string | null;
    editTitle?: string | null;
    editIcon?: IconName;
    editIconStyle?: IconStyle;
    editDisabled?: boolean;
    hasBottomMargin?: boolean;
    required?: boolean;
    error?: FieldError | Merge<FieldError, FieldErrorsImpl<T>> | null;
    onEditClick?: () => void;
}

export default function FormField<T extends object>({
    children,
    className,
    label,
    description,
    editHref,
    editTitle,
    editIcon,
    editIconStyle,
    editDisabled,
    hasBottomMargin,
    required,
    error,
    onEditClick
}: Props<T>) {
    let fieldClass = "FormField form-group";
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
                    <ActionLink href={editDisabled ? undefined : editHref}>
                        <Icon style={editIconStyle ?? "Solid"} name={editIcon ?? "pencil"} className="icon" title={editTitle ?? ""} />
                    </ActionLink>
                </div>
            )}
            {!editHref && onEditClick && (
                <div className="float-end">
                    <button
                        type="button"
                        title={editTitle ?? ""}
                        disabled={editDisabled}
                        className="btn btn-link m-0 p-0"
                        onClick={onEditClick}
                    >
                        <Icon style="Solid" name={editIcon ?? "pencil"} />
                    </button>
                </div>
            )}
            {label && (
                <label className="form-label" title={errorTooltip ?? undefined}>
                    {label}
                    {(required || error) && (
                        <sup className="text-danger ms-1">
                            <Icon style="Regular" name="asterisk" className="fa-xs" />
                        </sup>
                    )}
                    {error && (
                        <sup className="text-danger">
                            <Icon style="Regular" name="exclamation" className="ms-1" />
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
