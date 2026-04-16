import { shiftConfig } from "@/helpers/shiftConfig";
import { urlHelper } from "@/helpers/urlHelper";
import { HTMLAttributeAnchorTarget, ReactNode } from "react";
import { Link } from "react-router";
import { IconStyle } from "./icon/IconStyle";
import { IconName } from "./icon/IconName";
import Icon from "./icon/Icon";

interface Props {
    href?: string;
    className?: string;
    target?: HTMLAttributeAnchorTarget;
    title?: string;
    tabIndex?: number;
    icon?: {
        style: IconStyle;
        name: IconName;
        className?: string;
    },
    enforceHttpRedirect?: boolean;
    children?: ReactNode;
}

export default function ActionLink({
    href,
    className,
    target,
    title,
    tabIndex,
    icon,
    enforceHttpRedirect = false,
    children
}: Props)
{
    if (!enforceHttpRedirect && href?.toLowerCase()?.startsWith(shiftConfig.clientPrefix)) {
        return (
            <Link to={href} className={className} target={target} title={title} tabIndex={tabIndex}>
                {children ?? (icon && (
                    <Icon style={icon.style} name={icon.name} className={icon.className} />
                ))}
            </Link>
        );
    }
    return (
        <a href={href ? urlHelper.getActionUrl(href) : ""} className={className} target={target} title={title} tabIndex={tabIndex}>
            {children ?? (icon && (
                <Icon style={icon.style} name={icon.name} className={icon.className} />
            ))}
        </a>
    );
}