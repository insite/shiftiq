import "./IconButton.css";
import Icon from "../icon/Icon";
import { IconName } from "../icon/IconName";
import { IconStyle } from "../icon/IconStyle";
import { Spinner } from "react-bootstrap";
import ActionLink from "../ActionLink";

interface Props {
    title?: string;
    iconStyle: IconStyle;
    iconName: IconName;
    iconClassName?: string;
    disabled?: boolean;
    className?: string;
    isLoading?: boolean;
    href?: string;
    onClick?: (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

export default function IconButton({
    title,
    iconStyle,
    iconName,
    iconClassName,
    disabled,
    className,
    isLoading = false,
    href,
    onClick,
}: Props)
{
    if (href && !disabled && !isLoading) {
        return (
            <ActionLink
                title={title}
                className={`btn btn-link m-0 p-0 text-decoration-none IconButton ${className ?? ""}`}
                href={href}
            >
                <Icon style={iconStyle} name={iconName} className={iconClassName} />
            </ActionLink>
        );
    }

    return (
        <button
            type="button"
            title={title}
            disabled={disabled || isLoading}
            className={`btn btn-link m-0 p-0 text-decoration-none IconButton ${className ?? ""}`}
            onClick={onClick}
        >
            {isLoading ? (
                <Spinner animation="border" role="status" size="sm" />
            ) : (
                <Icon style={iconStyle} name={iconName} className={iconClassName} />
            )}
        </button>
    );
}