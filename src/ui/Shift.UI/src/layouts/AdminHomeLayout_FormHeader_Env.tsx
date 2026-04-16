import ActionLink from "@/components/ActionLink";
import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";

export default function AdminHomeLayout_FormHeader_Env() {
    const { siteSetting } = useSiteProvider();
    const theme = siteSetting.Environment.Color;
    const name = siteSetting.Environment.Name;

    const badge = (
        <ActionLink href="/ui/portal/platform/environments">
            <span className={`badge bg-${theme} fs-sm`}>
                {name}
            </span>
        </ActionLink>
    );

    return (
        siteSetting.Environment.Name.toLowerCase() !== "production" ? (
            <div className={`float-end text-${theme}`}>
                <small className="me-1">
                    <Icon style="solid" name="circle-info" className="me-2" />
                    Remember you are <strong>not</strong> working in a live version
                </small>
                {badge}
            </div>
        ) : (
            <div className="float-end">
                {badge}
            </div>
        )
    );
}