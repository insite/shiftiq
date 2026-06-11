import { SidebarState } from "@/models/SidebarState";
import { TestAccount } from "@/models/TestAccount";

const sidebarStateKey = "inSite.common.adminSidebar";
const tempAccountKey = "inSite.testing.tempAccount";
const overrideOriginKey = "inSite.testing.overrideOrigin";
const hiddenNotificationsKey = "inSite.dashboard.hiddenNotifications";

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
        localStorage.removeItem(overrideOriginKey);
    },

    getOverrideOrigin(): string | null {
        return localStorage.getItem(overrideOriginKey);
    },

    setOverrideOrigin(origin: string): void {
        localStorage.setItem(overrideOriginKey, origin);
    },

    getHiddenNotifications(): string[] {
        const value = localStorage.getItem(hiddenNotificationsKey);
        return value ? JSON.parse(value) : [];
    },

    setHiddenNotifications(notifications: string[]) {
        localStorage.setItem(hiddenNotificationsKey, JSON.stringify(notifications));
    },
}