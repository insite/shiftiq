import SessionTimer from "@/components/SessionTimer";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import AdminHomeLayout_NavigationGroups from "./AdminHomeLayout_NavigationGroups";
import Icon from "@/components/icon/Icon";
import "./AdminHomeLayout_SideMenuContent.css";
import { useSessionProvider } from "@/contexts/session/SessionProviderContext";
import { translate } from "@/helpers/translate";

export default function AdminHomeLayout_SideMenuContent() {
    const { siteSetting: {
        UserHostAddress: address,
        Environment: { Version: version }
    } } = useSiteProvider();

    const { timeLeftInMs, isBlinking } = useSessionProvider();

    return (
        <div data-simplebar className="AdminHomeLayout_SideMenuContent">
            <div className="admin-simplebar-content">
                
                <AdminHomeLayout_NavigationGroups />

                {isBlinking && (
                    <nav className="widget-nav nav nav-light flex-column compact-footer">
                        <div className="text-danger fs-sm mt-3 blink_me">
                            <Icon
                                style="regular"
                                name="stopwatch"
                                title={timeLeftInMs > 0 ? translate("Session is about to expire") : translate("Session is expired")}
                            />
                        </div>
                    </nav>
                )}

                <nav className="widget-nav nav nav-light flex-column hide-compact footer">
                    <div className="text-body-secondary fs-sm mt-3">
                        <div className="mb-1">
                            <Icon style="regular" name="stopwatch" className="me-2" />
                            Session <SessionTimer />
                        </div>
                        <div className="mb-1">
                            <Icon style="regular" name="network-wired" className="me-2" />
                            {address}
                        </div>
                        <div className="mb-1">
                            <Icon style="regular" name="code-commit" className="me-2" />
                            v{version.split(".").filter((_, index) => index <= 2).join(".")}
                        </div>
                    </div>
                </nav>

            </div>
        </div>
    );
}