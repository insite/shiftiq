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
    children?: ReactNode;
}

export default function FormTab({
    tab,
    icon,
    title,
    children
}: Props) {
    return (
        <Tab
            eventKey={tab}
            title={(
                <>
                    {icon && <Icon style={icon.style} name={icon.name} className="me-2" />}
                    {title}
                </>
            )}
        >
            {children}
        </Tab>
    );
}