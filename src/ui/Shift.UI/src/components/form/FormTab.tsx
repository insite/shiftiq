import { ReactNode } from "react";
import { Tab } from "react-bootstrap";

interface Props {
    tab: string;
    icon?: string;
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
                    {icon && <i className={`me-2 ${icon}`}></i>}
                    {title}
                </>
            )}
        >
            {children}
        </Tab>
    );
}