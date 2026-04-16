import { IconName } from "./IconName";
import { IconStyle } from "./IconStyle";

type IconStyleInfo = {
    [key in IconStyle]: string;
}

const _styles: IconStyleInfo = {
    "brands": "fa-brands",
    "regular": "fa-regular",
    "solid": "fa-solid",
    "light": "fa-light",
}

interface Props {
    name: IconName;
    style: IconStyle;
    className?: string;
    title?: string;
}

export default function Icon({
    name,
    style,
    className,
    title
}: Props) {
    return (
        <i
            className={`${_styles[style]} fa-${name} ${className ?? ""}`}
            title={title}
        ></i>
    )
}