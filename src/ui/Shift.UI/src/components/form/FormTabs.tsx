import { ReactNode, useState } from "react";
import { Tabs } from "react-bootstrap";
import Icon from "../icon/Icon";
import { ObjectIndexer } from "@/models/ObjectIndexer";
import { IconStyle } from "../icon/IconStyle";
import { IconName } from "../icon/IconName";

interface Props {
    defaultTab: string;
    className?: string;
    children?: ReactNode;
};

export default function FormTabs({
    defaultTab,
    className,
    children
}: Props) {
    const [selectedTab, setSelectedTab] = useState(defaultTab);

    // Tabs expects eventKey property to be specified
    const newChildren = Array.isArray(children)
        ? (
            children.map(element => (element !== null && typeof element === "object" ? {
                ...element,
                props: "props" in element && element.props !== null && typeof element.props === "object" ? {
                    ...element.props,
                    eventKey: "tab" in element.props ? element.props.tab : undefined,
                    title: <Title props={element.props} />
                } : undefined
            } : element))
        ) : (
            children !== null && typeof children === "object" ? {
                ...children,
                props: "props" in children && children.props !== null && typeof children.props === "object" ? {
                    ...children.props,
                    eventKey: "tab" in children.props ? children.props.tab : undefined,
                    title: <Title props={children.props} />
                } : undefined
            } : children
        );

    return (
        <Tabs
            activeKey={selectedTab}
            transition={false}
            className={className}
            onSelect={tab => setSelectedTab(tab!)}
        >
            {newChildren}
        </Tabs>
    )
}

interface TitleProps {
    props: unknown;
}

function Title({ props }: TitleProps) {
    const title = (props as ObjectIndexer)["title"] as ReactNode;
    const subtitle = (props as ObjectIndexer)["subtitle"] as string;

    const icon = (props as ObjectIndexer)["icon"] as {
        style: IconStyle;
        name: IconName;
    };

    return (
        <>
            {icon && <Icon style={icon.style} name={icon.name} className="me-2" />}
            {title}
            {subtitle && (
                <small className="text-body-secondary ms-1">
                    {subtitle}
                </small>
            )}
        </>
    );
}