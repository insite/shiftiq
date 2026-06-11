import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { recentLinksHelper } from "@/helpers/recentLinksHelper";
import { useEffect, useState } from "react";

export default function AdminHomeLayout_Navigation_RecentMenu() {
    const { siteSetting } = useSiteProvider();
    const [items, setItems] = useState(() => recentLinksHelper.getAll(siteSetting.RecentLinksKey));

    useEffect(() => {
        setItems(recentLinksHelper.getAll(siteSetting.RecentLinksKey));
    }, [siteSetting.RecentLinksKey]);

    if (items.length === 0) {
        return null;
    }

    return (
        <li className="nav-item dropdown fs-sm">
            <a
                href="#"
                className="nav-link dropdown-toggle"
                data-bs-toggle="dropdown"
                data-bs-auto-close="outside"
                aria-expanded="false"
            >
                <Icon style="regular" name="circle-bookmark" className="fa-width-auto me-2" />
                Recent
            </a>

            <ul className="dropdown-menu">
                {items.map(item => (
                    <li key={item.key}>
                        <a className="dropdown-item" href={item.pageUrl}>
                            {item.pageTitle || item.observerName}
                        </a>
                    </li>
                ))}
            </ul>
        </li>
    );
}
