import { Fragment } from "react/jsx-runtime";
import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/SiteProvider";

export default function AdminHomeLayout_NavigationGroups() {
    const { siteSetting } = useSiteProvider();
    return (
        siteSetting.NavigationGroups.map(group => (
            <Fragment key={group.Title}>
                <h3 className="h6 text-light pt-2 pb-2 border-bottom border-light compact-h3">
                    <span className="hide-compact">
                        {group.Title}
                    </span>
                </h3>
                <ul className="nav flex-column">
                    {group.MenuItems.map(item => (
                        <li key={item.Url} className="nav-item">
                            <ActionLink className="nav-link text-light fs-sm" href={item.Url} title={item.Text}>
                                <i className={`me-2 ${item.Icon}`}></i>
                                <span className="hide-compact">
                                    {item.Text}
                                </span>
                            </ActionLink>
                        </li>
                    ))}
                </ul>
            </Fragment>
        ))
    );
}
