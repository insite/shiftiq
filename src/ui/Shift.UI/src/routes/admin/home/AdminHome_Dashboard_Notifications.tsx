import Icon from "@/components/icon/Icon";
import { ActiveNotification } from "./ActiveNotification";
import { dateTimeHelper } from "@/helpers/date/dateTimeHelper";
import { ApiDashboardNotificationType } from "@/api/controllers/platform/dashboard/ApiDashboardNotificationType";
import { localStorageHelper } from "@/helpers/localStorageHelper";
import { useEffect, useRef, useState } from "react";

interface Props {
    notifications: ActiveNotification[] | null | undefined;
}

interface VisibleNotification {
    notification: ActiveNotification;
    hideState: "visible" | "preparing" | "hiding";
    height: number | null;
}

interface NotificationRowStyle extends React.CSSProperties {
    "--dashboard-notification-height"?: string;
    "--dashboard-notification-transition-duration"?: string;
}

const notificationHideTransitionDurationMs = 250;

export default function AdminHome_Dashboard_Notifications({ notifications }: Props) {
    const [visibleNotifications, setVisibleNotifications] = useState(() => filterHiddenNotifications(notifications));

    const notificationRowRefs = useRef<Record<string, HTMLDivElement | null>>({});
    const hideStartTimeoutsRef = useRef<Record<string, number>>({});
    const hideFallbackTimeoutsRef = useRef<Record<string, number>>({});

    useEffect(() => {
        setVisibleNotifications(filterHiddenNotifications(notifications));
    }, [notifications]);

    useEffect(() => {
        const hideStartTimeouts = hideStartTimeoutsRef.current;
        const hideFallbackTimeouts = hideFallbackTimeoutsRef.current;

        return () => {
            Object.values(hideStartTimeouts).forEach(timeoutId => clearTimeout(timeoutId));
            Object.values(hideFallbackTimeouts).forEach(timeoutId => clearTimeout(timeoutId));
        };
    }, []);

    if (visibleNotifications.length === 0) {
        return null;
    }

    function handleHideClick(notificationId: string) {
        if (hideStartTimeoutsRef.current[notificationId] || hideFallbackTimeoutsRef.current[notificationId]) {
            return;
        }

        const rowElement = notificationRowRefs.current[notificationId];
        const rowHeight = rowElement?.getBoundingClientRect().height ?? rowElement?.scrollHeight ?? 0;

        setVisibleNotifications(current => current.map(row =>
            row.notification.notificationId === notificationId && row.hideState === "visible"
                ? {
                    ...row,
                    hideState: "preparing",
                    height: rowHeight,
                }
                : row
        ));

        hideStartTimeoutsRef.current[notificationId] = window.setTimeout(() => {
            delete hideStartTimeoutsRef.current[notificationId];

            setVisibleNotifications(current => current.map(row =>
                row.notification.notificationId === notificationId && row.hideState === "preparing"
                    ? {
                        ...row,
                        hideState: "hiding",
                    }
                    : row
            ));

            hideFallbackTimeoutsRef.current[notificationId] = window.setTimeout(() => {
                finalizeHiddenNotification(notificationId);
            }, notificationHideTransitionDurationMs + 50);
        }, 0);
    }

    function finalizeHiddenNotification(notificationId: string) {
        clearHideTimeouts(notificationId);
        const newNotifications = hideNotification(notifications, notificationId);
        setVisibleNotifications(newNotifications);
    }

    function clearHideTimeouts(notificationId: string) {
        if (hideStartTimeoutsRef.current[notificationId]) {
            clearTimeout(hideStartTimeoutsRef.current[notificationId]);
            delete hideStartTimeoutsRef.current[notificationId];
        }

        if (hideFallbackTimeoutsRef.current[notificationId]) {
            clearTimeout(hideFallbackTimeoutsRef.current[notificationId]);
            delete hideFallbackTimeoutsRef.current[notificationId];
        }
    }

    function handleNotificationTransitionEnd(notificationId: string) {
        const notification = visibleNotifications.find(row => row.notification.notificationId === notificationId);
        if (notification?.hideState !== "hiding") {
            return;
        }

        finalizeHiddenNotification(notificationId);
    }

    function getNotificationRowStyle(notification: VisibleNotification): NotificationRowStyle {
        const style: NotificationRowStyle = {
            "--dashboard-notification-transition-duration": `${notificationHideTransitionDurationMs}ms`,
        };

        if (notification.height !== null) {
            style["--dashboard-notification-height"] = `${notification.height}px`;
        }

        return style;
    }

    return (
        <>
            <h3 className="mt-4">What's happening this week</h3>

            <div className="card border-1 shadow mb-3">
                <div className="card-body notification-grid">

                    {visibleNotifications.map(({ notification, hideState, height }) => (
                        <div
                            key={notification.notificationId}
                            ref={element => { notificationRowRefs.current[notification.notificationId] = element }}
                            className={`dashboard-notification${hideState === "hiding" ? " hiding" : ""}`}
                            style={getNotificationRowStyle({ notification, hideState, height })}
                            onTransitionEnd={() => handleNotificationTransitionEnd(notification.notificationId)}
                        >
                            {getNotificationIcon(notification.type)}
                            <div className="dashboard-notification-body">
                                <strong>{notification.title}</strong>
                                {notification.details && <span className="ms-2">{notification.details}</span>}
                                {notification.linkText && notification.linkUrl && (
                                    <a target="_blank" href={notification.linkUrl} className="ms-2">
                                        {notification.linkText}
                                    </a>
                                )}
                                <div className="form-text">
                                    Last updated: {dateTimeHelper.formatDate(notification.modified.date, "mmmm d, yyyy")}
                                </div>
                            </div>
                            <button
                                type="button"
                                className="btn btn-sm btn-default dashboard-notification-hide"
                                title="Hide"
                                disabled={hideState !== "visible"}
                                onClick={() => handleHideClick(notification.notificationId)}
                            >
                                <Icon style="regular" name="x" />
                            </button>
                        </div>
                    ))}
                    
                </div>
            </div>
        </>
    );
}

function filterHiddenNotifications(notifications: ActiveNotification[] | null | undefined): VisibleNotification[] {
    if (!notifications || notifications.length === 0) {
        return [];
    }
    const hiddenIds = localStorageHelper.getHiddenNotifications();
    return createVisibleNotifications(notifications.filter(({ notificationId }) => !hiddenIds.includes(notificationId)));
}

function hideNotification(notifications: ActiveNotification[] | null | undefined, hideId: string): VisibleNotification[] {
    if (!notifications || notifications.length === 0) {
        return [];
    }

    const hiddenIds = localStorageHelper.getHiddenNotifications();
    if (!hiddenIds.includes(hideId)) {
        hiddenIds.push(hideId);
    }

    const newHiddenIds = hiddenIds.filter(id => notifications.find(({ notificationId }) => notificationId === id));
    localStorageHelper.setHiddenNotifications(newHiddenIds);
    return createVisibleNotifications(notifications.filter(({ notificationId }) => !newHiddenIds.includes(notificationId)));
}


function createVisibleNotifications(notifications: ActiveNotification[]): VisibleNotification[] {
    return notifications.map(notification => ({
        notification,
        hideState: "visible",
        height: null,
    }));
}

function getNotificationIcon(type: ApiDashboardNotificationType) {
    switch (type) {
        case "PlatformUpdate":
            return (
                <div className="dashboard-notification-icon bg-warning">
                    <Icon style="light" name="refresh" />
                </div>
            );
        case "ReleaseNotes":
            return (
                <div className="dashboard-notification-icon bg-success">
                    <Icon style="light" name="rocket" />
                </div>
            );
        case "Other":
            return (
                <div className="dashboard-notification-icon bg-dark">
                    <Icon style="light" name="volume-high" />
                </div>
            );
        default:
            throw new Error(`Unknown type: ${type}`);
    }
}
