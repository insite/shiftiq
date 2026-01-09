import { CacheKey } from "./cacheKey";

export interface SearchSettingItem {
    title: string;
    data: object;
}

export interface SearchSettingList {
    items: SearchSettingItem[];
    selectedId: string | null;
}

type Subkey = "criteria" | "criteriaName" | "savedCriteriaList" | "download" | "downloadName" | "savedDownloadList" | "pageIndex";

function set(key: CacheKey, subkey: Subkey, value: unknown | null | undefined) {
    const path = `${key}.${subkey}`;

    if (!value) {
        localStorage.removeItem(path);
        return;
    }

    const serialized = typeof value === "string" ? value : JSON.stringify(value);

    localStorage.setItem(path, serialized);
}

function get(key: CacheKey, subkey: Subkey): string | null {
    const path = `${key}.${subkey}`;
    return localStorage.getItem(path);
}

function deserialize<T>(key: CacheKey, subkey: Subkey): T | null {
    const json = get(key, subkey);
    return json ? JSON.parse(json) as T : null;
}

function setPageIndex(key: CacheKey, pageIndex: number) {
    set(key, "pageIndex", String(pageIndex));
}

function getPageIndex(key: CacheKey | null | undefined): number | null {
    const value = key ? get(key, "pageIndex") : null;
    return value ? Number(value) : null;
}

class SearchStorageManager {
    constructor (
        private itemKey: Subkey,
        private itemNameKey: Subkey,
        private itemListKey: Subkey
    ) {}

    load(key: CacheKey): SearchSettingList {
        return {
            items: deserialize<SearchSettingItem[]>(key, this.itemListKey) ?? [],
            selectedId: get(key, this.itemNameKey),
        };
    }

    save(key: CacheKey, title: string | null, data: object): SearchSettingList {
        set(key, this.itemKey, data);

        if (!title) {
            return this.load(key);
        }

        const savedItem: SearchSettingItem = {
            title,
            data
        };

        let list = deserialize<SearchSettingItem[]>(key, this.itemListKey);
        if (!list) {
            list = [savedItem];
        } else {
            list = [...list.filter(x => x.title.toLowerCase() !== title.toLowerCase()), savedItem];
        }

        set(key, this.itemListKey, list);
        set(key, this.itemNameKey, title);

        return this.load(key);
    }

    select(key: CacheKey, title: string | null): SearchSettingList {
        const list = title ? deserialize<SearchSettingItem[]>(key, this.itemListKey) : null;
        const savedItem = title && list ? list.find(x => x.title.toLowerCase() === title.toLowerCase()) : null;
        
        set(key, this.itemNameKey, savedItem ? title : null);
        set(key, this.itemKey, savedItem?.data);

        return this.load(key);
    }

    delete(key: CacheKey, title: string): SearchSettingList {
        let list = deserialize<SearchSettingItem[]>(key, this.itemListKey);
        if (!list) {
            return this.load(key);
        }

        list = [...list.filter(x => x.title.toLowerCase() !== title.toLowerCase())];

        set(key, this.itemListKey, list.length ? list : null);
        set(key, this.itemNameKey, null);

        return this.load(key);
    }

    getDefaultItem<T>(key: CacheKey | null | undefined): T | null {
        if (!key) {
            return null;
        }
        
        const title = get(key, this.itemNameKey);
        if (!title) {
            return deserialize<T>(key, this.itemKey);
        }
        
        const list = deserialize<SearchSettingItem[]>(key, this.itemListKey);
        return list ? list.find(x => x.title.toLowerCase() === title.toLowerCase())?.data as T ?? null : null;
    }
}

export const searchStorage = {
    setPageIndex,
    getPageIndex,

    criteriaManager: new SearchStorageManager("criteria", "criteriaName", "savedCriteriaList"),
    downloadManager: new SearchStorageManager("download", "downloadName", "savedDownloadList"),
}