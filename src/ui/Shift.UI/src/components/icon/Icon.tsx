import { IconName } from "./IconName";
import { IconStyle } from "./IconStyle";

type IconStyleInfo = {
    [key in IconStyle]: string;
}

const _styles: IconStyleInfo = {
    "Brands": "fa-brands",
    "Regular": "fa-regular",
    "Solid": "fa-solid",
    "Light": "fa-light",
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