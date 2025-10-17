import { ReactNode } from "react";

export type AlertType = "none" | "error" | "information" | "success" | "warning";

interface Props {
    alertType?: AlertType;
    icon?: string | null;
    children?: ReactNode;
    className?: string;
    messages?: ReactNode[];
}

export default function Alert({ alertType, icon, children, className, messages }: Props) {
    const alertInfo = getAlertInfo(alertType ?? "none");
    
    const alertIcon = icon !== undefined
        ? (icon ? icon : "")
        : `far fa-${alertInfo.icon}`;

    return (
        <div role="alert" className={`alert alert-${alertInfo.variant} d-flex alert-custom-padding ${className ?? ""}`}>
            {alertInfo.icon && (
                <i className={`${alertIcon} fs-xl me-2`}></i>
            )}
            {messages ? (
                <>
                    {messages.length > 1 ? (
                        <ul>
                            {messages.map((e, index) => <li key={index}>{e}</li>)}
                        </ul>
                    ) : (
                        <>{messages[0]}</>
                    )}
                </>
            ) : (
                <div>{children}</div>
            )}
        </div>
    );
}

function getAlertInfo(alertType: AlertType): { variant: string; icon: string | null } {
    switch (alertType) {
        case "none":
            return { variant: "light", icon: null };
        case "error":
            return { variant: "danger", icon: "stop-circle" };
        case "information":
            return { variant: "info", icon: "info-square" };
        case "success":
            return { variant: "success", icon: "check-circle" };
        case "warning":
            return { variant: "warning", icon: "exclamation-triangle" };
        default:
            throw new Error(`Invalid alert: ${alertType}`);
    }
}