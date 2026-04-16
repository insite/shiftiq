import "./IconButton.css";
import Icon from "../icon/Icon";
import { IconName } from "../icon/IconName";
import { IconStyle } from "../icon/IconStyle";

interface Props {
    title?: string;
    iconStyle: IconStyle;
    iconName: IconName;
    iconClassName?: string;
    disabled?: boolean;
    className?: string;
    onClick?: (e: React.MouseEvent<HTMLButtonElement, MouseEvent>) => void;
}

export default function IconButton({
    title,
    iconStyle,
    iconName,
    iconClassName,
    disabled,
    className,
    onClick,
}: Props)
{
    return (
        <button
            type="button"
            title={title}
            disabled={disabled}
            className={`btn btn-link m-0 p-0 text-decoration-none IconButton ${className ?? ""}`}
            onClick={onClick}
        >
            <Icon style={iconStyle} name={iconName} className={iconClassName} />
        </button>
    );
}