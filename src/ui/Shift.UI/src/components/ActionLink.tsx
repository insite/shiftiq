import { shiftConfig } from "@/helpers/shiftConfig";
import { urlHelper } from "@/helpers/urlHelper";
import { HTMLAttributeAnchorTarget, ReactNode } from "react";
import { Link } from "react-router";

interface Props {
    href?: string;
    className?: string;
    target?: HTMLAttributeAnchorTarget;
    title?: string;
    tabIndex?: number;
    children?: ReactNode;
}

export default function ActionLink({ href, className, target, title, tabIndex, children }: Props) {
    if (href?.toLowerCase()?.startsWith(shiftConfig.clientPrefix)) {
        return (
            <Link to={href} className={className} target={target} title={title} tabIndex={tabIndex}>
                {children}
            </Link>
        );
    }
    return (
        <a href={href ? urlHelper.getActionUrl(href) : ""} className={className} target={target} title={title} tabIndex={tabIndex}>
            {children}
        </a>
    );
}