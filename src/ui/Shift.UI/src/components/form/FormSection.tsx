import { ReactNode } from "react";
import FormCard from "./FormCard";

interface Props {
    className?: string;
    title?: string;
    icon?: string;
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
                    {icon && <i className={`${icon} me-3`}></i>}
                    {title}
                </h2>
            )}
            <FormCard>
                {children}
            </FormCard>
        </section>
    )
}