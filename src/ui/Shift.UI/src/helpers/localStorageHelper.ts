import { SidebarState } from "@/models/SidebarState";
import { TestAccount } from "@/models/TestAccount";

const sidebarStateKey = "inSite.common.adminSidebar";
const tempAccountKey = "inSite.testing.tempAccount";

export const localStorageHelper = {
    getSidebarState(): SidebarState {
        const value = localStorage.getItem(sidebarStateKey);
        return value === "close" ? "collapsed" : "expanded";
    },

    setSidebarState(state: SidebarState): void {
        const value = state === "collapsed" ? "close" : "open";
        localStorage.setItem(sidebarStateKey, value);
    },

    getTempAccount(): TestAccount | null {
        const value = localStorage.getItem(tempAccountKey);
        return value ? JSON.parse(value) : null;
    },

    setTempAccount(account: TestAccount): void {
        localStorage.setItem(tempAccountKey, JSON.stringify(account));
    }
}