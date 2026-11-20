import AdminHomeLayout_UserMenu from "./AdminHomeLayout_UserMenu";
import AdminHomeLayout_DesktopMenu from "./AdminHomeLayout_DesktopMenu";
import AdminHomeLayout_MobileMenu from "./AdminHomeLayout_MobileMenu";
import { useState } from "react";
import ActionLink from "@/components/ActionLink";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";
import { shiftConfig } from "@/helpers/shiftConfig";
import { SidebarState } from "../models/SidebarState";
import Icon from "@/components/icon/Icon";
import AdminHomeLayout_Navigation_AdminMenu from "./AdminHomeLayout_Navigation_AdminMenu";
import AdminHomeLayout_Navigation_HelpMenu from "./AdminHomeLayout_Navigation_HelpMenu";
import AdminHomeLayout_Navigation_Lang from "./AdminHomeLayout_Navigation_Lang";
import AdminHomeLayout_Navigation_RecentMenu from "./AdminHomeLayout_Navigation_RecentMenu";
import AdminHomeLayout_Navigation_Search from "./AdminHomeLayout_Navigation_Search";

function getStateClass(state: SidebarState): string {
    switch (state) {
        case "collapsing1":
            return "compact-move";
        case "collapsing2":
            return "compact-move compact-size";
        case "collapsed":
            return "compact-size";
        case "expanding1":
        case "expanding2":
            return "compact-move";
        case "expanded":
            return "";
        default:
            throw new Error(`Unsupported state: ${state}`);
    }
}

interface Props {
    state: SidebarState;
    onStateChange: (state: SidebarState) => void;
}

export default function AdminHomeLayout_Navigation({ state, onStateChange }: Props) {
    const [show, setShow] = useState(false);
    const { siteSetting } = useSiteProvider();

    const handleToggleMobile = () => setShow(!show);
    const handleHideMobile = () => setShow(false);

    return (
        <>
            <header className="navbar admin-navbar navbar-expand-lg justify-content-end fixed-top shadow-sm bg-white py-0 px-3 px-lg-4" data-scroll-header="">
                <div className="container-fluid">

                    <div className="navbar-brand d-lg-none"></div>

                    <button type="button" className="navbar-toggler" aria-label="Toggle navigation" onClick={handleToggleMobile}>
                        <span className="navbar-toggler-icon"></span>
                    </button>

                    <AdminHomeLayout_Navigation_Search className={getStateClass(state)} />

                    <nav className="collapse navbar-collapse">
                        <ul className="navbar-nav ms-auto">

                            {shiftConfig.isLocal && (
                                <>
                                    <li className="nav-item fs-sm">
                                        <ActionLink className="nav-link text-info" href="/client/react/home">
                                            <Icon style="brands" name="react" className="fa-width-auto me-2" />
                                            React {siteSetting.OrganizationCode ? `(${siteSetting.OrganizationCode})` : ""}
                                        </ActionLink>
                                    </li>
                                    <li className="nav-item fs-sm">
                                        <div className="nav-link disabled text-border px-1">|</div>
                                    </li>
                                </>
                            )}

                            <AdminHomeLayout_Navigation_RecentMenu />

                            <li className="nav-item fs-sm">
                                <ActionLink className="nav-link" href="/ui/portal/home">
                                    <Icon style="regular" name="chalkboard-user" className="fa-width-auto me-2" />
                                    Portal
                                </ActionLink>
                            </li>

                            <AdminHomeLayout_Navigation_AdminMenu />

                            <AdminHomeLayout_UserMenu />

                            {siteSetting.ImpersonatorName && (
                                <>
                                    <li className="nav-item fs-sm">
                                        <ActionLink className="nav-link text-danger" href="/ui/portal/security/impersonation/stop">
                                            <Icon style="regular" name="user-secret" className="fa-width-auto me-2" />
                                            {siteSetting.ImpersonatorName}
                                        </ActionLink>
                                    </li>
                                </>
                            )}

                            {siteSetting.UserName && (
                                <AdminHomeLayout_Navigation_HelpMenu />
                            )}

                            <AdminHomeLayout_Navigation_Lang />
                        </ul>
                    </nav>
                </div>
            </header>

            <AdminHomeLayout_DesktopMenu onStateChange={onStateChange} />

            <AdminHomeLayout_MobileMenu show={show} onHide={handleHideMobile} />
        </>
    );
}
