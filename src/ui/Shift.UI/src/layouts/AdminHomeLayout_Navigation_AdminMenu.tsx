import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { IconName } from "@/components/icon/IconName";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";

export default function AdminHomeLayout_Navigation_AdminMenu() {
    const { siteSetting } = useSiteProvider();

    if (siteSetting.AdminNavigationGroups.length === 0) {
        return null;
    }

    return (
        <li className="nav-item dropdown fs-sm">

            <a href="#"
                className="nav-link dropdown-toggle"
                data-bs-toggle="dropdown"
                data-bs-auto-close="outside"
                aria-expanded="false"
            >
                <Icon style="regular" name="grid-round" className="me-1 fa-width-auto" />
                Admin
            </a>

            <div className="dropdown-menu dropdown-menu-end p-0">
                <div className="d-lg-flex">

                    {siteSetting.AdminNavigationGroups.map(({ Title, MenuItems }) => (
                        <div key={Title} className="mega-dropdown-column pt-1 pt-lg-3 pb-lg-4">
                            <ul className="list-unstyled mb-0">
                                <li><span className="fs-lg ms-3 text-secondary">{Title}</span></li>
                                {MenuItems.map(item => (
                                    <li key={item.Url}>
                                        <ActionLink className="dropdown-item" href={item.Url}>
                                            <Icon style="regular" name={item.Icon as IconName} className="me-1" />
                                            {item.Text}
                                        </ActionLink>
                                    </li>
                                ))}
                            </ul>
                        </div>    
                    ))}

                </div>
            </div>

        </li>
    )
}