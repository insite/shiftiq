import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/SiteProvider";

export default function AdminHomeLayout_UserMenu() {
    const { siteSetting } = useSiteProvider();
    
    if (!siteSetting.UserName) {
        return null;
    }

    return (
        <li className="nav-item dropdown fs-sm">
                    
            <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                <Icon style="Regular" name="user" className="fa-width-auto me-2" />
                {siteSetting.UserName}
            </a>

            <ul className="dropdown-menu">
                {siteSetting.MyDashboard && (
                    <li>
                        <ActionLink className="dropdown-item" href={siteSetting.MyDashboard.Url}>
                            {siteSetting.MyDashboard.Text}
                        </ActionLink>
                    </li>
                )}
                <li>
                    <ActionLink className="dropdown-item" href="/ui/portal/profile">My Profile</ActionLink>
                </li>
                {siteSetting.IsAdministrator && (
                    <li>
                        <ActionLink className="dropdown-item" href="/ui/portal/platform/environments">Select Environment</ActionLink>
                    </li>
                )}
                {siteSetting.IsMultiOrganization && (
                    <li>
                        <ActionLink className="dropdown-item" href="/ui/portal/security/organizations?auto-redirect=0">Select Organization</ActionLink>
                    </li>
                )}
                <li>
                    <ActionLink className="dropdown-item" href="/ui/portal/identity/password">Change Password</ActionLink>
                </li>
                <li>
                    <ActionLink className="dropdown-item" href="/ui/lobby/signout">Sign Out</ActionLink>
                </li>
            </ul>

        </li>
    );
}