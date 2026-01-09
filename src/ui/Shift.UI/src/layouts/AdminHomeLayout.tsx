import { Outlet } from "react-router";
import ScrollTopButton from "@/components/ScrollTopButton";
import SiteProvider from "@/contexts/SiteProvider";
import StatusProvider from "@/contexts/StatusProvider";
import LoadingContext from "@/contexts/LoadingProvider";
import AdminHomeLayout_Navigation from "./AdminHomeLayout_Navigation";
import AdminHomeLayout_FormHeader from "./AdminHomeLayout_FormHeader";

export default function AdminHomeLayout() {
    return (
        <SiteProvider>
            <AdminHomeLayout_Navigation />
            {/* <uc:CmdsNavigation runat="server" ID="CmdsNavigation" /> */}

            <main className="admin-container">
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
        </SiteProvider>
    );
}