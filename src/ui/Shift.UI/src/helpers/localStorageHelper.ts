import { SidebarState } from "@/models/SidebarState";

const sidebarStateKey = "inSite.common.adminSidebar";

export const localStorageHelper = {
    getSidebarState(): SidebarState {
        const value = localStorage.getItem(sidebarStateKey);
        return value === "close" ? "collapsed" : "expanded";
    },
    setSidebarState(state: SidebarState): void {
        const value = state === "collapsed" ? "close" : "open";
        localStorage.setItem(sidebarStateKey, value);
    }
}