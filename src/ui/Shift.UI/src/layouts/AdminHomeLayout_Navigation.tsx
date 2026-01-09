import AdminHomeLayout_UserMenu from "./AdminHomeLayout_UserMenu";
import AdminHomeLayout_DesktopMenu from "./AdminHomeLayout_DesktopMenu";
import AdminHomeLayout_MobileMenu from "./AdminHomeLayout_MobileMenu";
import { useState } from "react";
import AdminHomeLayout_EnvItem from "./AdminHomeLayout_EnvItem";
import ActionLink from "@/components/ActionLink";
import AdminHomeLayout_PermissionItem from "./AdminHomeLayout_PermissionItem";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { shiftConfig } from "@/helpers/shiftConfig";

export default function AdminHomeLayout_Navigation() {
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
                    
                    <nav className="collapse navbar-collapse">
                        <ul className="navbar-nav ms-auto">

                            {shiftConfig.isLocal && (
                                <>
                                    <li className="nav-item">
                                        <ActionLink className="nav-link text-info" href="/client/react/home">
                                            <i className="fab fa-width-auto fa-react me-2"></i>React {siteSetting.OrganizationCode ? `(${siteSetting.OrganizationCode})` : ""}
                                        </ActionLink>
                                    </li>
                                    <li className="nav-item">
                                        <div className="nav-link disabled text-border px-1">|</div>
                                    </li>
                                </>
                            )}

                            {siteSetting.IsCmds && (
                                <li className="nav-item">
                                    <ActionLink className="nav-link" href={siteSetting.CmdsHomeLink}><i className="fas fa-width-auto fa-boxes me-2"></i>CMDS</ActionLink>
                                </li>
                            )}
                            <li className="nav-item">
                                <ActionLink className="nav-link" href="/ui/portal/home"><i className="fas fa-width-auto fa-layer-group me-2"></i>Portal</ActionLink>
                            </li>
                            <li className="nav-item d-none">
                                <ActionLink className="d-none nav-link" href="/client/admin/home"><i className="fas fa-width-auto fa-cog me-2"></i>Admin</ActionLink>
                            </li>
                            <li className="nav-item">
                                <div className="nav-link disabled text-border px-1">|</div>
                            </li>

                            <AdminHomeLayout_UserMenu />

                            <li className="nav-item">
                                <div className="nav-link disabled text-border px-1">|</div>
                            </li>

                            {siteSetting.ImpersonatorName && (
                                <>
                                    <li className="nav-item">
                                        <ActionLink className="nav-link text-danger" href="/ui/portal/accounts/users/impersonate">
                                            <i className="fas fa-width-auto fa-user-secret me-2"></i>
                                            {siteSetting.ImpersonatorName}
                                        </ActionLink>
                                    </li>
                                    <li className="nav-item">
                                        <div className="nav-link disabled text-border px-1">|</div>
                                    </li>
                                </>
                            )}

                            {siteSetting.UserName && (
                                <>
                                    <li className="nav-item">
                                        <ActionLink className="nav-link" href="/ui/portal/support" target="_blank"><i className="fas fa-width-auto fa-envelope me-2"></i>Support</ActionLink>
                                    </li>
                                    <li className="nav-item">
                                        <div className="nav-link disabled text-border px-1">|</div>
                                    </li>
                                </>
                            )}
                    

                            <AdminHomeLayout_PermissionItem />

                            <AdminHomeLayout_EnvItem />
                        </ul>
                    </nav>
                </div>
            </header>

            <AdminHomeLayout_DesktopMenu />

            <AdminHomeLayout_MobileMenu show={show} onHide={handleHideMobile} />
        </>
    );
}