import { useEffect, useState } from "react";
import { Outlet, useLocation } from "react-router";
import ScrollTopButton from "@/components/ScrollTopButton";
import SiteProvider, { useSiteProvider } from "@/contexts/SiteProvider";
import StatusProvider from "@/contexts/StatusProvider";
import LoadingContext from "@/contexts/LoadingProvider";
import AdminHomeLayout_Navigation from "./AdminHomeLayout_Navigation";
import AdminHomeLayout_FormHeader from "./AdminHomeLayout_FormHeader";
import { SidebarState } from "../models/SidebarState";
import { localStorageHelper } from "@/helpers/localStorageHelper";

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

export default function AdminHomeLayout() {
    return (
        <SiteProvider>
            <AdminHomeLayoutInternal />
        </SiteProvider>
    );
}

function AdminHomeLayoutInternal() {
    const [state, setState] = useState<SidebarState>(localStorageHelper.getSidebarState());

    const location = useLocation();

    const { setActionSubtitle } = useSiteProvider();

    useEffect(() => {
        setActionSubtitle(null);
    }, [location, setActionSubtitle]);

    return (
        <>
            <AdminHomeLayout_Navigation onStateChange={setState} />

            <main className={`admin-container ${getStateClass(state)}`}>
                <div className="row pb-3 pb-md-4">
                    <div className="col-xxl-12">
                        <AdminHomeLayout_FormHeader />
                        <LoadingContext>
                            <StatusProvider>
                                <Outlet />
                            </StatusProvider>
                        </LoadingContext>
                    </div>
                </div>
            </main>

            <ScrollTopButton />

            <div id="bottom"></div>
        </>
    );
}