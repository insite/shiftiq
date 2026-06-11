export interface RecentLinkItem {
    pageUrl: string;
    observerName: string;
    pageTitle: string | null;
    key: string;
}

function getStorageKey(recentLinksKey: string): string {
    return `recentLinks.${recentLinksKey}`;
}

function isRecentLinkItem(value: unknown): value is RecentLinkItem {
    if (!value || typeof value !== "object") {
        return false;
    }

    const item = value as Record<string, unknown>;

    return typeof item.pageUrl === "string"
        && item.pageUrl.length > 0
        && typeof item.observerName === "string"
        && item.observerName.length > 0
        && (typeof item.pageTitle === "string" || item.pageTitle === null)
        && typeof item.key === "string"
        && item.key.length > 0;
}

export const recentLinksHelper = {
    getStorageKey(recentLinksKey: string): string {
        return getStorageKey(recentLinksKey);
    },

    getAll(recentLinksKey: string | null | undefined): RecentLinkItem[] {
        if (!recentLinksKey) {
            return [];
        }

        try {
            const value = localStorage.getItem(getStorageKey(recentLinksKey));
            if (!value) {
                return [];
            }

            const data = JSON.parse(value);
            if (!Array.isArray(data)) {
                return [];
            }

            return data.filter(isRecentLinkItem);
        } catch {
            return [];
        }
    },
};
