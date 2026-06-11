import SearchResult from "@/components/search/SearchResult";
import { NotificationRow } from "./NotificationRow";
import { translate } from "@/helpers/translate";
import ActionLink from "@/components/ActionLink";
import DateTimeField from "@/components/DateTimeField";
import IconButton from "@/components/iconbutton/IconButton";

export default function NotificationSearch_Result() {
    return (
        <SearchResult<NotificationRow> columns={[
            {
                key: "edit",
                className: "edit-one-tool",
                item: row => (
                    <IconButton
                        iconStyle="solid"
                        iconName="pencil"
                        title={translate("Edit Notification")}
                        href={`/client/admin/platform/notifications/edit/${row.notificationId}`}
                    />
                )
            },
            {
                key: "type",
                title: translate("Type"),
                item: row => row.type === "PlatformUpdate"
                    ? "Platform Update"
                    : row.type === "ReleaseNotes"
                        ? "Release Notes"
                        : "Other"
            },
            {
                key: "title",
                title: translate("Title"),
                itemClassName: "text-nowrap",
                item: row => (
                    <ActionLink href={`/client/admin/platform/notifications/edit/${row.notificationId}`}>
                        {row.title}
                    </ActionLink>
                )
            },
            {
                key: "dateRange",
                title: translate("Date Range"),
                item: row => (
                    <>
                        {row.startDate && !row.endDate && (
                            <>
                                <span className="form-text me-2">from</span>
                                <DateTimeField dateTime={row.startDate} />
                            </>
                        )}
                        {!row.startDate && row.endDate && (
                            <>
                                <span className="form-text me-2">until</span>
                                <DateTimeField dateTime={row.endDate} />
                            </>
                        )}
                        {row.startDate && row.endDate && (
                            <>
                                <DateTimeField dateTime={row.startDate} />
                                <span className="ms-2 me-2">-</span>
                                <DateTimeField dateTime={row.endDate} />
                            </>
                        )}
                    </>
                )
            },
            {
                key: "isActive",
                className: "text-center",
                title: translate("Active?"),
                item: row => row.isActive ? "Yes" : "No"
            },
            {
                key: "visibleOnDashboard",
                className: "text-center",
                title: translate("Visible on Dashboard?"),
                item: row => row.visibleOnDashboard ? "Yes" : "No"
            },
            {
                key: "created",
                title: translate("Created"),
                itemClassName: "text-nowrap",
                item: row => (
                    <>
                        {row.createdByName}<br/>
                        <DateTimeField dateTime={row.created} />
                    </>
                )
            },
            {
                key: "modified",
                title: translate("Modified"),
                itemClassName: "text-nowrap",
                item: row => (
                    <>
                        {row.modifiedByName}<br/>
                        <DateTimeField dateTime={row.modified} />
                    </>
                )
            },
        ]} />
    );
}