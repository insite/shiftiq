import AdminHomeLayout_HomeLink from "./AdminHomeLayout_HomeLink";
import AdminHomeLayout_SideMenuContent from "./AdminHomeLayout_SideMenuContent";

export default function AdminHomeLayout_DesktopMenu() {
    return (
        <aside id="DesktopMenu" className="position-fixed d-none d-lg-block admin-sidebar">
            <div className="admin-sidebar-header">
                <h5 className="m-0 d-inline-block">
                    <AdminHomeLayout_HomeLink />
                </h5>
            </div>

            <div className="admin-simplebar-wrapper" style={{ height: "calc(100% - 2.9rem - 0.25rem)" }}>
                <AdminHomeLayout_SideMenuContent />
            </div>
        </aside>
    );
}