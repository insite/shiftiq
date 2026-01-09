import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/SiteProvider";

export default function AdminHomeLayout_PermissionItem() {
    const { siteSetting } = useSiteProvider();
    if (!siteSetting.Permissions
        || !siteSetting.Permissions.Accounts && !siteSetting.Permissions.Integrations && !siteSetting.Permissions.Settings
    )
    {
        return;
    }
    return (
        <li className="nav-item dropdown">
            <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false"><i className="fas fa-width-auto fa-cogs me-2"></i>Utilities</a>

            <ul className="dropdown-menu">
                {siteSetting.Permissions.Settings && (
                    <li><ActionLink className="dropdown-item" href="/ui/admin/setup/home">Platform</ActionLink></li>
                )}
                {siteSetting.Permissions.Accounts && (
                    <li><ActionLink className="dropdown-item" href="/ui/admin/security/home">Security</ActionLink></li>
                )}
                {siteSetting.Permissions.Accounts && (
                    <li><ActionLink className="dropdown-item" href="/ui/admin/timeline/home">Timeline</ActionLink></li>
                )}
                {siteSetting.Permissions.Integrations && (
                    <li><ActionLink className="dropdown-item" href="/ui/admin/integration/home">Integration</ActionLink></li>
                )}
            </ul>
        </li>
    );
}