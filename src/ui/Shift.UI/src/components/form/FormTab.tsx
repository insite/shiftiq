import { ReactNode } from "react";
import { Tab } from "react-bootstrap";
import { IconName } from "../icon/IconName";
import Icon from "../icon/Icon";
import { IconStyle } from "../icon/IconStyle";
import { numberHelper } from "@/helpers/numberHelper";

interface Props {
    tab: string;
    icon?: {
        style: IconStyle;
        name: IconName;
    };
    title: ReactNode;
    subtitle?: string;
    count?: number;
    children?: ReactNode;
}

export default function FormTab({
    tab,
    icon,
    title,
    subtitle,
    count,
    children
}: Props) {
    return (
        <Tab
            eventKey={tab}
            title={(
                <>
                    {icon && <Icon style={icon.style} name={icon.name} className="me-2" />}
                    {title}
                    {subtitle ? (
                        <small className="text-body-secondary ms-1">
                            {subtitle}
                        </small>
                    ) : count !== undefined ? (
                        <small className="text-body-secondary ms-1">
                            {`(${numberHelper.formatInt(count)})`}
                        </small>
                    ) : null}
                </>
            )}
        >
            {children}
        </Tab>
    );
}