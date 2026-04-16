import { ReactNode } from "react";
import { Tab } from "react-bootstrap";
import { IconName } from "../icon/IconName";
import Icon from "../icon/Icon";
import { IconStyle } from "../icon/IconStyle";

interface Props {
    tab: string;
    icon?: {
        style: IconStyle;
        name: IconName;
    };
    title: ReactNode;
    subtitle?: string;
    children?: ReactNode;
}

export default function FormTab({
    tab,
    icon,
    title,
    subtitle,
    children
}: Props) {
    return (
        <Tab
            eventKey={tab}
            title={(
                <>
                    {icon && <Icon style={icon.style} name={icon.name} className="me-2" />}
                    {title}
                    {subtitle && (
                        <small className="text-body-secondary ms-1">
                            {subtitle}
                        </small>
                    )}
                </>
            )}
        >
            {children}
        </Tab>
    );
}