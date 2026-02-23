import SessionTimer from "@/components/SessionTimer";
import { useSiteProvider } from "@/contexts/SiteProvider";
import AdminHomeLayout_NavigationGroups from "./AdminHomeLayout_NavigationGroups";
import Icon from "@/components/icon/Icon";
import "./AdminHomeLayout_SideMenuContent.css";

export default function AdminHomeLayout_SideMenuContent() {
    const { siteSetting: {
        UserHostAddress: address,
        Environment: { Version: version }
    } } = useSiteProvider();

    return (
        <div data-simplebar className="AdminHomeLayout_SideMenuContent">
            <div className="admin-simplebar-content">
                
                <AdminHomeLayout_NavigationGroups />
                            
                <nav className="widget-nav nav nav-light flex-column hide-compact footer">
                    <div className="text-body-secondary fs-sm mt-3">
                        <div className="mb-1">
                            <Icon style="Regular" name="stopwatch" className="me-2" />
                            Session <SessionTimer />
                        </div>
                        <div className="mb-1">
                            <Icon style="Regular" name="network-wired" className="me-2" />
                            {address}
                        </div>
                        <div className="mb-1">
                            <Icon style="Regular" name="code-commit" className="me-2" />
                            v{version.split(".").filter((_, index) => index <= 2).join(".")}
                        </div>
                    </div>
                </nav>

            </div>
        </div>
    );
}