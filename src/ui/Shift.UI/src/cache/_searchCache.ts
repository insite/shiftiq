import { SelectedTab } from "@/models/enums";
import { CacheKey } from "./cacheKey";
import { BaseCriteria } from "@/components/search/BaseCriteria";

interface Data {
    pageIndex: number;
    criteria: BaseCriteria | null;
    rows: object[] | null;
    totalRowCount: number;
    rowsPerPage: number;
}

interface Search {
    selectedTab: SelectedTab,
    data: Data | null;
}

const _map: Map<CacheKey, Search> = new Map();

export const _searchCache = {
    clear() {
        _map.clear();
    },

    setSelectedTab(key: CacheKey, selectedTab: SelectedTab) {
        const search = _map.get(key);

        if (search) {
            search.selectedTab = selectedTab;
        } else {
            _map.set(key, {
                selectedTab,
                data: null,
            });
        }
    },

    setData(key: CacheKey, data: Data) {
        const search = _map.get(key);

        if (search) {
            search.data = data;
        } else {
            _map.set(key, {
                selectedTab: "result",
                data,
            });
        }
    },

    clearRows(key: CacheKey) {
        const search = _map.get(key);

        if (search?.data) {
            search.data.rows = null;
        }
    },

    get<Criteria extends BaseCriteria, Row>(key: CacheKey) {
        const search = _map.get(key);
        if (!search) {
            return null;
        }

        const data = search?.data
            ? {
                ...search.data,
                criteria: search.data.criteria as Criteria,
                rows: search.data.rows as Row[],
            }: null;

        return {
            selectedTab: search.selectedTab,
            data,
        };
    },

    delete(key: CacheKey) {
        _map.delete(key);
    },
}