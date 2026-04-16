import { useState } from "react";
import { Outlet } from "react-router";
import ScrollTopButton from "@/components/ScrollTopButton";
import SiteProvider from "@/contexts/site/SiteProvider";
import StatusProvider from "@/contexts/status/StatusProvider";
import LoadingProvider from "@/contexts/loading/LoadingProvider";
import AdminHomeLayout_Navigation from "./AdminHomeLayout_Navigation";
import AdminHomeLayout_FormHeader from "./AdminHomeLayout_FormHeader";
import { SidebarState } from "../models/SidebarState";
import { localStorageHelper } from "@/helpers/localStorageHelper";
import SessionProvider from "@/contexts/session/SessionProvider";
import PageProvider from "@/contexts/page/PageProvider";

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
    const [state, setState] = useState<SidebarState>(localStorageHelper.getSidebarState());
    return (
        <SiteProvider>
            <SessionProvider>
                <PageProvider>

                    <AdminHomeLayout_Navigation onStateChange={setState} />

                    <main className={`admin-container ${getStateClass(state)}`}>
                        <div className="row pb-3 pb-md-4">
                            <div className="col-xxl-12">
                                <AdminHomeLayout_FormHeader />
                                <LoadingProvider>
                                    <StatusProvider>
                                        <Outlet />
                                    </StatusProvider>
                                </LoadingProvider>
                            </div>
                        </div>
                    </main>

                    <ScrollTopButton />

                    <div id="bottom"></div>

                </PageProvider>
            </SessionProvider>
        </SiteProvider>
    );
}