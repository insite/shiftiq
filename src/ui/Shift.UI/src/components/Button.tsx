import { translate } from "@/helpers/translate";
import { MouseEvent, ReactNode } from "react";
import { Spinner } from "react-bootstrap";
import ActionLink from "./ActionLink";
import { IconName } from "./icon/IconName";
import Icon from "./icon/Icon";

type ButtonType = "button" | "submit" | "reset";

type Variant =
    "add"
    | "new"
    | "delete"
    | "delete-icon"
    | "clear"
    | "search"
    | "save"
    | "icon-save"
    | "cancel"
    | "close"
    | "request"
    | "download"
    | "reset"
    | "download-excel"
    | "lock"
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
    }
}

const variants: ButtonVariantList = {
    add: {
        variantIcon: "plus-circle",
        variantTitle: "Add",
        variantLoadingMessage: "Adding...",
        variantClass: "default"
    },
    new: {
        variantIcon: "file",
        variantTitle: "New",
        variantLoadingMessage: "Creating...",
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
    clear: {
        variantIcon: "times",
        variantTitle: "Clear",
        variantLoadingMessage: "Clearing...",
        variantClass: "primary"
    },
    search: {
        variantIcon: "search",
        variantTitle: "Search",
        variantLoadingMessage: "Searching...",
        variantClass: "primary"
    },
    save: {
        variantIcon: "save",
        variantTitle: "Save",
        variantLoadingMessage: "Saving...",
        variantClass: "success"
    },
    "icon-save": {
        variantIcon: "save",
        variantTitle: "",
        variantLoadingMessage: "",
        variantClass: "default btn-icon"
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
    request: {
        variantIcon: "globe-pointer",
        variantTitle: "API Request",
        variantLoadingMessage: "Requesting...",
        variantClass: "default"
    },
    download: {
        variantIcon: "download",
        variantTitle: "Download",
        variantLoadingMessage: "Downloading...",
        variantClass: "default"
    },
    reset: {
        variantIcon: "undo",
        variantTitle: "Reset",
        variantLoadingMessage: "Resetting...",
        variantClass: "primary"
    },
    "download-excel": {
        variantIcon: "file-excel",
        variantTitle: "Download",
        variantLoadingMessage: "Downloading...",
        variantClass: "default"
    },
    lock: {
        variantIcon: "lock",
        variantTitle: "Lock",
        variantLoadingMessage: "Locking...",
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
    const { variantIcon, variantTitle, variantLoadingMessage, variantClass, variantOnlyIcon } = variants[variant];

    let icon: ReactNode;
    let content: string;

    if (isLoading) {
        content = loadingMessage ?? translate(variantLoadingMessage) ?? translate("Loading...");
        icon = <Spinner animation="border" role="status" size="sm" className={!variantOnlyIcon && content ? "me-2" : ""} />;
    } else {
        content = text || translate(variantTitle);
        icon = <Icon style="Solid" name={variantIcon} className={!variantOnlyIcon && content ? "me-2" : ""} />;
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
                {icon}
                {!variantOnlyIcon ? content : null}
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
            {icon}
            {!variantOnlyIcon ? content : null}
        </button>
    );
}
