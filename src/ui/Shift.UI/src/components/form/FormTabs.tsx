import { ReactNode, useState } from "react";
import { Tabs } from "react-bootstrap";

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
                    eventKey: "tab" in element.props ? element.props.tab : undefined
                } : undefined
            } : element))
        ) : (
            children !== null && typeof children === "object" ? {
                ...children,
                props: "props" in children && children.props !== null && typeof children.props === "object" ? {
                    ...children.props,
                    eventKey: "tab" in children.props ? children.props.tab : undefined
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