import { translate } from "@/helpers/translate";
import { MouseEvent, ReactNode } from "react";
import { Spinner } from "react-bootstrap";
import ActionLink from "./ActionLink";

type ButtonType = "button" | "submit" | "reset";

type Variant = "new" | "delete" | "clear" | "search" | "save" | "icon-save" | "cancel" | "request" | "download" | "reset" | "download-excel" |
    "lock" | "unlock";

interface Props {
    variant: Variant;
    text?: string;
    title?: string;
    type?: ButtonType;
    className?: string;
    disabled?: boolean;
    tabIndex?: number;
    isLoading?: boolean;
    loadingMessage?: ReactNode;
    href?: string;
    onClick?: (e: MouseEvent<HTMLButtonElement>) => void;
}

interface ButtonVariantList {
    [variant: string]: {
        variantIcon: string;
        variantTitle: string;
        variantLoadingMessage: string;
        variantClass: string;
    }
}

const variants: ButtonVariantList = {
    new: {
        variantIcon: "fas fa-file",
        variantTitle: "New",
        variantLoadingMessage: "Creating...",
        variantClass: "default"
    },
    delete: {
        variantIcon: "fas fa-trash-alt",
        variantTitle: "Delete",
        variantLoadingMessage: "Deleting...",
        variantClass: "danger"
    },
    clear: {
        variantIcon: "fas fa-times",
        variantTitle: "Clear",
        variantLoadingMessage: "Clearing...",
        variantClass: "primary"
    },
    search: {
        variantIcon: "fas fa-search",
        variantTitle: "Search",
        variantLoadingMessage: "Searching...",
        variantClass: "primary"
    },
    save: {
        variantIcon: "fas fa-save",
        variantTitle: "Save",
        variantLoadingMessage: "Saving...",
        variantClass: "success"
    },
    "icon-save": {
        variantIcon: "fas fa-save",
        variantTitle: "",
        variantLoadingMessage: "",
        variantClass: "default btn-icon"
    },
    cancel: {
        variantIcon: "fas fa-ban",
        variantTitle: "Cancel",
        variantLoadingMessage: "Cancelling...",
        variantClass: "default"
    },
    request: {
        variantIcon: "fas fa-globe-pointer",
        variantTitle: "API Request",
        variantLoadingMessage: "Requesting...",
        variantClass: "default"
    },
    download: {
        variantIcon: "fas fa-download",
        variantTitle: "Download",
        variantLoadingMessage: "Downloading...",
        variantClass: "default"
    },
    reset: {
        variantIcon: "fas fa-undo",
        variantTitle: "Reset",
        variantLoadingMessage: "Resetting...",
        variantClass: "primary"
    },
    "download-excel": {
        variantIcon: "fas fa-file-excel",
        variantTitle: "Download",
        variantLoadingMessage: "Downloading...",
        variantClass: "default"
    },
    lock: {
        variantIcon: "fas fa-lock",
        variantTitle: "Lock",
        variantLoadingMessage: "Locking...",
        variantClass: "default"
    },
    unlock: {
        variantIcon: "fas fa-lock-open",
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
    const { variantIcon, variantTitle, variantLoadingMessage, variantClass } = variants[variant];

    let icon: ReactNode;
    let content: ReactNode;

    if (isLoading) {
        content = loadingMessage ?? translate(variantLoadingMessage) ?? translate("Loading...");
        icon = <Spinner animation="border" role="status" size="sm" className={content ? "me-2" : ""} />;
    } else {
        content = text || translate(variantTitle);
        icon = <i className={`${variantIcon} ${content ? "me-2" : ""}`}></i>;
    }

    const finalClassName = `btn btn-sm btn-${variantClass} ${className ?? ""}`;

    if (href && !disabled && !isLoading) {
        return (
            <ActionLink
                title={title}
                className={finalClassName}
                tabIndex={tabIndex}
                href={href}
            >
                {icon}
                {content}
            </ActionLink>
        )
    }

    return (
        <button
            type={type ?? "submit"}
            title={title}
            className={finalClassName}
            disabled={disabled || isLoading}
            tabIndex={tabIndex}
            onClick={onClick}
        >
            {icon}
            {content}
        </button>
    );
}
