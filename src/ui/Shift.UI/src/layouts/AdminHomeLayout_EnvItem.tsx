import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { environmentHelper } from "@/helpers/environmentHelper";

export default function AdminHomeLayout_EnvItem() {
    const { siteSetting } = useSiteProvider();

    if (!siteSetting.Environment?.Name) {
        return;
    }

    const { indicator, icon } = environmentHelper.getIndicator(siteSetting.Environment.Name);

    return (
        <li className="nav-item">
            <ActionLink className={`nav-link text-nowrap text-${indicator}`} href="/ui/portal/settings/environments/select">
                <i className={`fas fa-width-auto fa-${icon} align-middle mt-n1 me-2`}></i>{siteSetting.Environment.Name}
            </ActionLink>
        </li>
    );
}