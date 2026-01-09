import { useSiteProvider } from "@/contexts/SiteProvider";
import AdminHome_PanelTile from "./AdminHome_PanelTile";

export default function AdminHome() {
    const { siteSetting } = useSiteProvider();

    const menuItems = siteSetting.NavigationGroups?.length
        ? siteSetting.NavigationGroups
            .map(g => g.MenuItems)
            .flat()
            .sort((a, b) => a.Text.localeCompare(b.Text))
        : null;

    return (
        <>
            {menuItems && (
                <section className="pb-4 mb-md-2">
                    <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-5 g-4">
                        {menuItems.map(({ Text: title, Url: url, Icon: icon }) => (
                            <AdminHome_PanelTile key={url} title={title} url={url} icon={icon} isShortcut={false} />
                        ))}
                    </div>
                </section>
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