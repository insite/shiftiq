import AdminHomeLayout_HomeLink from "./AdminHomeLayout_HomeLink";
import { Offcanvas } from "react-bootstrap";
import AdminHomeLayout_SideMenuContent from "./AdminHomeLayout_SideMenuContent";
import ActionLink from "@/components/ActionLink";

interface Props {
    show: boolean;    
    onHide: () => void;
}

export default function AdminHomeLayout_MobileMenu({ show, onHide }: Props) {
    return (
        <Offcanvas id="MobileMenu" show={show} onHide={onHide} className="admin-offcanvas">
            <div className="admin-offcanvas-header">
                <div>
                    <h5 className="offcanvas-title">
                        <AdminHomeLayout_HomeLink />
                    </h5>
                    <button className="btn-close btn-close-white" type="button" onClick={onHide}></button>
                </div>
            </div>
            <div className="d-flex align-items-center py-4 px-3 border-bottom border-light">
                <ActionLink className="btn btn-outline-light w-100 btn-xs me-2" href="/ui/portal/home">Portal</ActionLink>
                <ActionLink className="btn btn-outline-light w-100 btn-xs mt-0" href="/ui/lobby/signout">Sign Out</ActionLink>
            </div>
            <div className="admin-simplebar-wrapper" style={{ height: "calc(100% - 7.3rem - 0.25rem)" }}>
                <AdminHomeLayout_SideMenuContent />
            </div>
        </Offcanvas>
    );
}