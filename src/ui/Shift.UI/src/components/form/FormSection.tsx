import { ReactNode } from "react";
import FormCard from "./FormCard";
import { IconName } from "../icon/IconName";
import Icon from "../icon/Icon";

interface Props {
    className?: string;
    title?: string;
    icon?: IconName;
    children?: ReactNode;
}

export default function FormSection({
    className,
    title,
    icon,
    children
}: Props) {
    return (
        <section className={className}>
            {title && (
                <h2>
                    {icon && <Icon style="Solid" name={icon} className="me-3" />}
                    {title}
                </h2>
            )}
            <FormCard>
                {children}
            </FormCard>
        </section>
    )
}