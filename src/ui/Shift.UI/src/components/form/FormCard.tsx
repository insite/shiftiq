import { ReactNode } from "react";

interface Props {
    title?: ReactNode;
    children?: ReactNode;
    className?: string;
    bodyClassName?: string;
    hasShadow?: boolean;
    hasBottomMargin?: boolean;
}

export default function FormCard({
    title,
    children,
    className,
    bodyClassName,
    hasShadow,
    hasBottomMargin
}: Props) {
    let mergedClassName = "card";

    if (hasShadow === undefined || hasShadow) {
        mergedClassName += " border-0 shadow-lg";
    }
    if (hasBottomMargin === undefined || hasBottomMargin) {
        mergedClassName += " mb-3";
    }
    if (className) {
        mergedClassName += " " + className;
    }

    return (
        <div className={mergedClassName}>
            <div className={`card-body ${bodyClassName ?? ""}`}>
                {title && (
                    <h4 className="card-title">{title}</h4>
                )}
                {children}
            </div>
        </div>
    );
}