import { Fragment } from "react/jsx-runtime";
import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/SiteProvider";

export default function AdminHomeLayout_NavigationGroups() {
    const { siteSetting } = useSiteProvider();
    return (
        siteSetting.NavigationGroups.map(group => (
            <Fragment key={group.Title}>
                <h3 className="h6 text-light pt-2 pb-2 border-bottom border-light">
                    {group.Title}
                </h3>
                <ul className="nav flex-column">
                    {group.MenuItems.map(item => (
                        <li key={item.Url} className="nav-item">
                            <ActionLink className="nav-link fs-sm" href={item.Url}>
                                <i className={`me-2 ${item.Icon}`}></i>
                                {item.Text}
                            </ActionLink>
                        </li>
                    ))}
                </ul>
            </Fragment>
        ))
    );
}
