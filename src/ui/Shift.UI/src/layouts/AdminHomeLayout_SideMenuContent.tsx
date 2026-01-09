import ActionLink from "@/components/ActionLink";
import SessionTimer from "@/components/SessionTimer";
import { useSiteProvider } from "@/contexts/SiteProvider";
import { urlHelper } from "@/helpers/urlHelper";
import AdminHomeLayout_NavigationGroups from "./AdminHomeLayout_NavigationGroups";

export default function AdminHomeLayout_SideMenuContent() {
    const { siteSetting } = useSiteProvider();
    return (
        <div data-simplebar>
            <div className="admin-simplebar-content">
                
                <AdminHomeLayout_NavigationGroups />
                            
                <nav className="widget-nav nav nav-light flex-column">
                    <div className="pt-3">
                        <ActionLink href="/client/admin/home" className="navbar-brand py-1 flex-shrink-0">
                            <img src={urlHelper.getResourceUrl(siteSetting.PlatformLogoSrc)} style={{ maxHeight: "70px" }} alt="Shift iQ" title={`v${siteSetting.Environment.Version}`} />
                        </ActionLink>
                    </div>

                    <div className="text-body-secondary fs-sm mt-3">
                        <div className="mb-1">
                            <i className="far fa-stopwatch me-2"></i>Session <SessionTimer />
                        </div>
                        <div className="mb-1">
                            <i className="far fa-globe me-2"></i>{siteSetting.UserHostAddress}
                        </div>
                        <div className="mb-1">
                            <i className="far fa-copyright me-2"></i>{new Date().getFullYear()} InSite
                        </div>
                    </div>
                </nav>

            </div>
        </div>
    );
}