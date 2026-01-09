import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/SiteProvider";

export default function AdminHomeLayout_UserMenu() {
    const { siteSetting } = useSiteProvider();
    return (
        <li className="nav-item dropdown">
                    
            {siteSetting.UserName && (
                <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                    <i className='fas fa-width-auto fa-user me-2'></i>
                    {siteSetting.UserName}
                </a>
            )}

            <ul className="dropdown-menu">
                {siteSetting.MyDashboard && (
                    <li>
                        <ActionLink className="dropdown-item" href={siteSetting.MyDashboard.Url}>
                            {siteSetting.MyDashboard.Text}
                        </ActionLink>
                    </li>
                )}
                <li>
                    <ActionLink className="dropdown-item" href="/ui/portal/identity/profile">My Profile</ActionLink>
                </li>
                {siteSetting.IsAdministrator && (
                    <li>
                        <ActionLink className="dropdown-item" href="/ui/portal/platform/environments">Select Environment</ActionLink>
                    </li>
                )}
                {siteSetting.IsMultiOrganization && (
                    <li>
                        <ActionLink className="dropdown-item" href="/ui/portal/identity/organizations?auto-redirect=0">Select Organization</ActionLink>
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