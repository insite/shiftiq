import { useCallback, useState } from "react";
import AdminHomeLayout_HomeLink from "./AdminHomeLayout_HomeLink";
import AdminHomeLayout_SideMenuContent from "./AdminHomeLayout_SideMenuContent";
import { SidebarState } from "../models/SidebarState";
import { localStorageHelper } from "@/helpers/localStorageHelper";
import Icon from "@/components/icon/Icon";

const _opacityStepDurationMs = 150;
const _sizeStepDurationMs = 300;

function getStateClass(state: SidebarState): string {
    switch (state) {
        case "collapsing1":
            return "admin-sidebar-compact compact-hiding";
        case "collapsing2":
        case "collapsed":
            return "admin-sidebar-compact compact-hidden compact-size";
        case "expanding1":
            return "admin-sidebar-compact compact-hidden";
        case "expanding2":
            return "admin-sidebar-compact";
        case "expanded":
            return "";
        default:
            throw new Error(`Unsupported state: ${state}`);
    }
}

interface Props {
    onStateChange: (state: SidebarState) => void;
}

export default function AdminHomeLayout_DesktopMenu({ onStateChange }: Props) {
    const [state, setState] = useState<SidebarState>(localStorageHelper.getSidebarState());

    const setStateAndTriggerEvent = useCallback((state: SidebarState) => {
        setState(state);
        onStateChange(state);
    }, [onStateChange]);

    function onToggleClick() {
        switch (state) {
            case "expanded":
                setStateAndTriggerEvent("collapsing1");
                setTimeout(stepCollapsing1, _opacityStepDurationMs);
                break;
            case "collapsed":
                setStateAndTriggerEvent("expanding1");
                setTimeout(stepExpanding1, _sizeStepDurationMs);
                break;
            default:
                break;
        }
    }

    function stepExpanding1() {
        setStateAndTriggerEvent("expanding2");
        setTimeout(stepExpanding2, _opacityStepDurationMs);
    }

    function stepExpanding2() {
        setStateAndTriggerEvent("expanded");
        localStorageHelper.setSidebarState("expanded");
    }

    function stepCollapsing1() {
        setStateAndTriggerEvent("collapsing2");
        setTimeout(stepCollapsing2, _sizeStepDurationMs);
    }

    function stepCollapsing2() {
        setStateAndTriggerEvent("collapsed");
        localStorageHelper.setSidebarState("collapsed");
    }

    return (
        <aside
            id="DesktopMenu"
            className={`position-fixed d-none d-lg-block admin-sidebar ${getStateClass(state)}`}
        >
            <div className="admin-sidebar-header">
                <div className="admin-sidebar-toggle">
                    <div title="Toggle Sidebar" onClick={onToggleClick}>
                        <Icon style="solid" name="angle-left" className="fa-fw" />
                    </div>
                </div>
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