import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { IconName } from "@/components/icon/IconName";

const items: {
        title: string;
        iconName: IconName;
        href: string;
}[] = [
    {
        title: "Attendance Report",
        iconName: "clipboard-user",
        href: "/ui/admin/events/registrations/search",
    },
    {
        title: "Course Completions",
        iconName: "graduation-cap",
        href: "/ui/admin/records/credentials/search",
    },
    {
        title: "Grade Reports",
        iconName: "user-graduate",
        href: "/ui/admin/records/gradebooks/search",
    },
    {
        title: "Custom Reports",
        iconName: "box-magnifying-glass",
        href: "/ui/admin/reporting",
    },
]

export default function AdminHome_Dashboard_ReportTile() {
    return (
        <div className="col-6 col-lg-3 d-flex">
            <div className="card border-1 shadow w-100 h-100">
                <div className="card-body d-flex flex-column align-items-center">
                    <h3 className="text-center">
                        Generate Reports
                    </h3>

                    <div className="action-list with-icons">

                        {items.map(({ title, iconName, href }) => (
                            <div key={href}>
                                <span>
                                    <Icon style="regular" name={iconName} className="fs-3" />
                                    {title}
                                </span>
                                <ActionLink href={href} className="btn btn-sm btn-outline-primary">
                                    View
                                </ActionLink>
                            </div>
                        ))}

                    </div>

                    <ActionLink href="/ui/admin/reporting" className="btn btn-sm btn-primary">
                        Go to Reports
                    </ActionLink>
                </div>
            </div>
        </div>
    )
}