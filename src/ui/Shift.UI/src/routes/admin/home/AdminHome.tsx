import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import AdminHome_PanelTile from "./AdminHome_PanelTile";
import AdminHome_Dashboard from "./AdminHome_Dashboard";
import { usePageProvider } from "@/contexts/page/PageProviderContext";
import { useEffect, useState } from "react";
import { Dashboard } from "./Dashboard";
import { useLoadAction } from "@/hooks/useLoadAction";
import { shiftClient } from "@/api/shiftClient";
import { dashboardAdapter } from "./dashboardAdapter";
import MaintenanceToast from "@/routes/_shared/toasts/MaintenanceToast";

export default function AdminHome() {
    const [dashboard, setDashboard] = useState<Dashboard | null>(null);

    const { siteSetting } = useSiteProvider();
    const { hideTitle } = usePageProvider();
    const { isLoaded, runLoad } = useLoadAction(load);

    useEffect(() => { runLoad() }, [runLoad]);

    useEffect(() => {
        if (siteSetting.UserName) {
            hideTitle();
        }
    }, [siteSetting, hideTitle]);

    async function load() {
        const apiDashboard = await shiftClient.dashboard.retrieveDashboard();
        if (apiDashboard) {
            const dashboard = dashboardAdapter.getDashboard(apiDashboard, siteSetting.TimeZoneId);
            setDashboard(dashboard);
        }
    }

    if (!isLoaded) {
        return null;
    }

    const frequentlyUsedApps = siteSetting.FrequentlyUsedApps.filter((_, index) => index < 5);

    const allApps = siteSetting.AllApps
            .map(g => g.MenuItems)
            .flat()
            .filter(x => !frequentlyUsedApps.find(y => y.Text === x.Text))
            .sort((a, b) => a.Text.localeCompare(b.Text));

    return (
        <>
            <MaintenanceToast />

            {siteSetting.UserName && dashboard && <AdminHome_Dashboard dashboard={dashboard} />}

            {frequentlyUsedApps.length > 0 && (
                <>
                    <h3>Frequently Used Apps</h3>

                    <section className="pb-4 mb-md-2">
                        <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-5 g-4">
                            {frequentlyUsedApps.map(({ Text: title, Url: url, Icon: icon }) => (
                                <AdminHome_PanelTile key={url} title={title} url={url} icon={icon} isShortcut={false} />
                            ))}
                        </div>
                    </section>
                </>
            )}

            {allApps.length > 0 && (
                <>
                    <h3>{frequentlyUsedApps.length > 0 ? "Other Apps" : "All Apps"}</h3>

                    <section className="pb-4 mb-md-2">
                        <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-5 g-4">
                            {allApps.map(({ Text: title, Url: url, Icon: icon }) => (
                                <AdminHome_PanelTile key={url} title={title} url={url} icon={icon} isShortcut={false} />
                            ))}
                        </div>
                    </section>
                </>
            )}

            {siteSetting.ShortcutGroups && siteSetting.ShortcutGroups.length > 0 && (
                <section className="pb-4 mb-md-2">
                    <h2 className="h4 mb-3">
                        {siteSetting.CompanyName}
                    </h2>

                    <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-5 g-4">
                        {siteSetting.ShortcutGroups.map(({ Text: title, Url: url, Icon: icon }) => (
                            <AdminHome_PanelTile key={url} title={title} url={url} icon={icon} isShortcut={true} />
                        ))}
                    </div>
                </section>
            )}
        </>
    );
}