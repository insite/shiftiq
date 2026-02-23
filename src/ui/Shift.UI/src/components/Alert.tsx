import { ReactNode } from "react";
import { IconName } from "./icon/IconName";
import Icon from "./icon/Icon";

export type AlertType = "none" | "error" | "information" | "success" | "warning";

interface Props {
    alertType?: AlertType;
    children?: ReactNode;
    className?: string;
    messages?: ReactNode[];
}

export default function Alert({ alertType, children, className, messages }: Props) {
    const alertInfo = getAlertInfo(alertType ?? "none");
    
    return (
        <div role="alert" className={`alert alert-${alertInfo.variant} d-flex alert-custom-padding ${className ?? ""}`}>
            {alertInfo.icon && (
                <Icon style="Regular" name={alertInfo.icon} className="fs-xl me-2" />
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

function getAlertInfo(alertType: AlertType): { variant: string; icon: IconName | null } {
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