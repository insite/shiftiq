import { useState } from "react";
import { CacheKey } from "@/cache/cacheKey";
import { cache } from "@/cache/cache";
import { SelectedTab } from "@/models/enums";
import { BaseCriteria, cleanCriteria, hydrateCriteria } from "./BaseCriteria";
import { searchStorage } from "@/cache/searchStorage";

interface Defaults<Criteria, Row> {
    selectedTab: SelectedTab;
    pageIndex: number;
    criteria: Criteria; 
    rows: Row[] | null;
    totalRowCount: number;
    rowsPerPage: number;
    setSelectedTab: (selectedTab: SelectedTab) => void;
    setData: (
        pageIndex: number,
        criteria: Criteria,
        rows: Row[],
        totalRowCount: number,
        rowsPerPage: number,
    ) => void;
}

export function useSearchDefaults<Criteria extends BaseCriteria, Row extends object>(
    cacheKey: CacheKey | undefined | null,
    defaultSelectedTab: SelectedTab | undefined | null,
    defaultPageIndex: number | undefined | null,
    defaultCriteria: Criteria,
): Defaults<Criteria, Row> {
    const [defaults] = useState(() => {
        const search = cacheKey ? cache.search.get<Criteria, Row>(cacheKey) : null;
        return {
            selectedTab: search?.selectedTab ?? defaultSelectedTab ?? "result",
            pageIndex: search?.data?.pageIndex ?? searchStorage.getPageIndex(cacheKey) ?? defaultPageIndex ?? 0,
            criteria: search?.data?.criteria ?? getStoredCriteria(cacheKey, defaultCriteria),
            rows: search?.data?.rows ?? null,
            totalRowCount: search?.data?.totalRowCount ?? 0,
            rowsPerPage: search?.data?.rowsPerPage ?? 0,
            
            setSelectedTab(selectedTab: SelectedTab) {
                if (cacheKey) {
                    cache.search.setSelectedTab(cacheKey, selectedTab);
                }
            },

            setData(
                pageIndex: number,
                criteria: Criteria,
                rows: Row[],
                totalRowCount: number,
                rowsPerPage: number,
            ) {
                if (cacheKey) {
                    cache.search.setData(cacheKey, {
                        pageIndex,
                        criteria,
                        rows,
                        totalRowCount,
                        rowsPerPage,
                    });
                    searchStorage.setPageIndex(cacheKey, pageIndex);
                    setStoredCriteria(cacheKey, criteria, defaultCriteria);
                }
            },
        };
    });
    return defaults;
}

function getStoredCriteria<Criteria>(cacheKey: CacheKey | undefined | null, defaultCriteria: Criteria): Criteria {
    const stored = searchStorage.criteriaManager.getDefaultItem<Criteria>(cacheKey);
    return hydrateCriteria(stored, defaultCriteria);
}

function setStoredCriteria(cacheKey: CacheKey, criteria: object, defaultCriteria: object) {
    const result = cleanCriteria(criteria, defaultCriteria);
    searchStorage.criteriaManager.save(cacheKey, null, result);
}