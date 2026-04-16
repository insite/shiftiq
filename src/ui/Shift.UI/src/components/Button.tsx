import { translate } from "@/helpers/translate";
import { MouseEvent, ReactNode } from "react";
import { Spinner } from "react-bootstrap";
import ActionLink from "./ActionLink";
import { IconName } from "./icon/IconName";
import Icon from "./icon/Icon";

type ButtonType = "button" | "submit" | "reset";

type Variant =
    "add"
    | "apply-filter"
    | "cancel"
    | "close"
    | "clear"
    | "delete"
    | "delete-icon"
    | "download"
    | "download-excel"
    | "icon-save"
    | "new"
    | "next"
    | "lock"
    | "previous"
    | "reset"
    | "request"
    | "save"
    | "search"
    | "top"
    | "unlock";

interface Props {
    variant: Variant;
    text?: string;
    title?: string;
    type?: ButtonType;
    className?: string;
    disabled?: boolean;
    tabIndex?: number;
    isLoading?: boolean;
    loadingMessage?: string;
    href?: string;
    onClick?: (e: MouseEvent<HTMLButtonElement>) => void;
}

type ButtonVariantList = {
    [variant in Variant]: {
        variantIcon: IconName;
        variantTitle: string;
        variantLoadingMessage: string;
        variantClass: string;
        variantOnlyIcon?: boolean;
        variantIsRightIcon?: boolean;
    }
}

const variants: ButtonVariantList = {
    add: {
        variantIcon: "plus-circle",
        variantTitle: "Add",
        variantLoadingMessage: "Adding...",
        variantClass: "default"
    },
    "apply-filter": {
        variantIcon: "filter",
        variantTitle: "Apply",
        variantLoadingMessage: "Applying...",
        variantClass: "default"
    },
    clear: {
        variantIcon: "times",
        variantTitle: "Clear",
        variantLoadingMessage: "Clearing...",
        variantClass: "primary"
    },
    cancel: {
        variantIcon: "ban",
        variantTitle: "Cancel",
        variantLoadingMessage: "Cancelling...",
        variantClass: "default"
    },
    close: {
        variantIcon: "ban",
        variantTitle: "Close",
        variantLoadingMessage: "Closing...",
        variantClass: "default"
    },
    delete: {
        variantIcon: "trash-alt",
        variantTitle: "Delete",
        variantLoadingMessage: "Deleting...",
        variantClass: "danger"
    },
    "delete-icon": {
        variantIcon: "trash-alt",
        variantTitle: "Delete",
        variantLoadingMessage: "Deleting...",
        variantClass: "default",
        variantOnlyIcon: true,
    },
    download: {
        variantIcon: "download",
        variantTitle: "Download",
        variantLoadingMessage: "Downloading...",
        variantClass: "default"
    },
    "download-excel": {
        variantIcon: "file-excel",
        variantTitle: "Download",
        variantLoadingMessage: "Downloading...",
        variantClass: "default"
    },
    "icon-save": {
        variantIcon: "save",
        variantTitle: "",
        variantLoadingMessage: "",
        variantClass: "default btn-icon"
    },
    lock: {
        variantIcon: "lock",
        variantTitle: "Lock",
        variantLoadingMessage: "Locking...",
        variantClass: "default"
    },
    new: {
        variantIcon: "file",
        variantTitle: "New",
        variantLoadingMessage: "Creating...",
        variantClass: "default"
    },
    next: {
        variantIcon: "arrow-alt-right",
        variantTitle: "Next",
        variantLoadingMessage: "Moving...",
        variantClass: "primary",
        variantIsRightIcon: true,
    },
    previous: {
        variantIcon: "arrow-alt-left",
        variantTitle: "Previous",
        variantLoadingMessage: "Moving...",
        variantClass: "default"
    },
    reset: {
        variantIcon: "undo",
        variantTitle: "Reset",
        variantLoadingMessage: "Resetting...",
        variantClass: "primary"
    },
    request: {
        variantIcon: "globe-pointer",
        variantTitle: "API Request",
        variantLoadingMessage: "Requesting...",
        variantClass: "default"
    },
    save: {
        variantIcon: "save",
        variantTitle: "Save",
        variantLoadingMessage: "Saving...",
        variantClass: "success"
    },
    search: {
        variantIcon: "search",
        variantTitle: "Search",
        variantLoadingMessage: "Searching...",
        variantClass: "primary"
    },
    top: {
        variantIcon: "arrow-up",
        variantTitle: "Top",
        variantLoadingMessage: "Moving...",
        variantClass: "default"
    },
    unlock: {
        variantIcon: "lock-open",
        variantTitle: "Unlock",
        variantLoadingMessage: "Unlocking...",
        variantClass: "default"
    },
};

export default function Button({
    variant,
    text,
    title,
    type,
    className,
    disabled,
    tabIndex,
    isLoading,
    loadingMessage,
    href,
    onClick
}: Props) {
    const { variantIcon, variantTitle, variantLoadingMessage, variantClass, variantOnlyIcon, variantIsRightIcon } = variants[variant];

    let icon: ReactNode;
    let content: string;

    if (isLoading) {
        content = loadingMessage ?? translate(variantLoadingMessage) ?? translate("Loading...");
        icon = <Spinner animation="border" role="status" size="sm" className={!variantOnlyIcon && content ? variantIsRightIcon ? "ms-2" : "me-2" : ""} />;
    } else {
        content = text || translate(variantTitle);
        icon = <Icon style="solid" name={variantIcon} className={!variantOnlyIcon && content ? variantIsRightIcon ? "ms-2" : "me-2" : ""} />;
    }

    const finalClassName = `btn btn-sm btn-${variantClass} ${variantOnlyIcon ? "btn-icon" : ""} ${className ?? ""}`;

    if (href && !disabled && !isLoading) {
        return (
            <ActionLink
                title={title || !variantOnlyIcon ? title : content}
                className={finalClassName}
                tabIndex={tabIndex}
                href={href}
            >
                {!variantIsRightIcon && icon}
                {!variantOnlyIcon ? content : null}
                {variantIsRightIcon && icon}
            </ActionLink>
        )
    }

    return (
        <button
            type={type ?? "submit"}
            title={title || !variantOnlyIcon ? title : content}
            className={finalClassName}
            disabled={disabled || isLoading}
            tabIndex={tabIndex}
            onClick={onClick}
        >
            {!variantIsRightIcon && icon}
            {!variantOnlyIcon ? content : null}
            {variantIsRightIcon && icon}
        </button>
    );
}
